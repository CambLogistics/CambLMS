namespace camblms.sites

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

open camblms.dto
open camblms.templating
open camblms.server.controller

[<JavaScript>]
module ServiceFeeAdmin =
    let OwedUserList = ListModel.Create (fun (u: Member) -> u) []
    let PendingFeeList = ListModel.Create (fun (f: PendingFee) -> f) []
    let SelectedOwedUserID = Var.Create -1
    let ImageUserList = ListModel.Create (fun (u: Member) -> u) []
    let ImageList = ListModel.Create (fun (i: string * int * System.DateTime) -> i) []
    let SelectedImageUploader = Var.Create -1

    let currentImages =
        View.Map (fun il -> il |> Seq.filter (fun (_, uid, _) -> uid = SelectedImageUploader.Value)) ImageList.View

    let sessionID = JavaScript.Cookies.Get("clms_sid").Value

    let updateImageLists () =
        async {
            let! userlist = UserController.getUserList sessionID false true
            let! imagelist = DocumentController.getImageList sessionID

            match imagelist with
            | Ok l ->
                ImageList.Set l

                match userlist with
                | Ok ul ->
                    query {
                        for (fn, uploader, _) in l do
                            join u in ul on (uploader = u.Id)
                            select u
                    }
                    |> ImageUserList.Set

                    if SelectedImageUploader.Value = -1 && not <| Seq.isEmpty ImageUserList.Value then
                        SelectedImageUploader.Set <| (Seq.head ImageUserList.Value).Id
                | Error e -> Feedback.giveFeedback true <| "Hiba a felhasználó lista lekérésekor: " + e
            | Error e -> Feedback.giveFeedback true <| "Hiba a képlista lekérésekor: " + e
        }
        |> Async.Start

    let updateOwedUserList () =
        async {
            let! list = UserController.getUserList sessionID false false

            match list with
            | Ok l ->
                OwedUserList.Set l
                SelectedOwedUserID.Set (Seq.item 0 OwedUserList.Value).Id
            | Error e -> Feedback.giveFeedback true <| "Hiba a felhasználók lekérdezésekor: " + e
        }
        |> Async.Start

    let updatePendingFeeList () =
        async {
            let! list = ServiceFeeController.getPendingFees sessionID

            match list with
            | Ok l ->
                PendingFeeList.Set l
                SelectedOwedUserID.Set (Seq.item 0 OwedUserList.Value).Id
            | Error e -> Feedback.giveFeedback true <| "Hiba a szervizdíjak lekérdezésekor: " + e
        }
        |> Async.Start

    let RenderPage () =
        updateOwedUserList ()
        updatePendingFeeList ()
        updateImageLists ()

        SiteParts
            .ServiceTemplate()
            .OwedMemberList(
                OwedUserList.View
                |> Doc.BindSeqCached(fun u ->
                    SiteParts.ServiceTemplate
                        .OwedMemberItem()
                        .UserID(string u.Id)
                        .Member(u.Name)
                        .Doc())
            )
            .MemberSelectID(SelectedOwedUserID.Lens (fun id -> string id) (fun id s -> int s))
            .ServiceList(
                PendingFeeList.View
                |> Doc.BindSeqCached(fun p ->
                    SiteParts.ServiceTemplate
                        .ServiceListItem()
                        .ID(string p.ID)
                        .Name(p.Username)
                        .Paid(fun e ->
                            ActionDispatcher.RunAction
                                ServiceFeeController.payFee
                                (sessionID, p.ID)
                                (Some updatePendingFeeList))
                        .ServiceFee((string p.Amount) + "$")
                        .Doc())
            )
            .Submit(fun e ->
                if JavaScript.JS.IsNaN e.Vars.Fee.Value then
                    Feedback.giveFeedback true "Kérlek számot adj meg árnak!"
                else
                    ActionDispatcher.RunAction
                        ServiceFeeController.submitPendingFee
                        (sessionID, (int e.Vars.MemberSelectID.Value), (int e.Vars.Fee.Value))
                        (Some updatePendingFeeList))
            .OnSelection(fun _ -> updateImageLists ())
            .UploaderList(
                Doc.BindSeqCached
                    (fun (u: Member) ->
                        SiteParts.ServiceTemplate
                            .UploaderItem()
                            .MemberID(string u.Id)
                            .MemberName(u.Name)
                            .Doc())
                    ImageUserList.View
            )
            .SelectedUploader(SelectedImageUploader.Lens (fun m -> string m) (fun m mi -> int mi))
            .ImageList(
                Doc.BindSeqCached
                    (fun (fn, uploader, (date: System.DateTime)) ->
                        SiteParts.ServiceTemplate
                            .ImageItem()
                            .ImageLink("/img/" + fn)
                            .UploadDate(
                                string date.Year
                                + "-"
                                + sprintf "%02d" date.Month
                                + "-"
                                + sprintf "%02d" date.Day
                                + " "
                                + sprintf "%02d" date.Hour
                                + ":"
                                + sprintf "%02d" date.Minute
                            )
                            .Delete(fun e ->
                                ActionDispatcher.RunAction
                                    DocumentController.deleteImage
                                    (sessionID, fn)
                                    (Some updateImageLists))
                            .Doc())
                    currentImages
            )
            .Doc()

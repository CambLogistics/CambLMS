namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module ImageAdmin =
    let userList = ListModel.Create (fun (u:Member) -> u) []
    let imageList = ListModel.Create (fun (i:string*int*System.DateTime) -> i) []
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
    let selectedMember = Var.Create -1
    let currentImages = View.Map (fun il -> il |> Seq.filter (fun (_,uid,_) -> uid = selectedMember.Value)) imageList.View
    let updateLists() =
        async{
            let! userlist = UserOperations.getUserList sessionID false true
            let! imagelist = ImageUpload.getImageList sessionID
            match imagelist with
                |Ok l ->
                    imageList.Set l
                    match userlist with
                        |Ok ul ->
                            query{
                                for (fn,uploader,_) in l do
                                join u in ul on (uploader = u.Id)
                                select u
                            } |> userList.Set
                            if selectedMember.Value = -1 && not <| Seq.isEmpty userList.Value then selectedMember.Set <| (Seq.head userList.Value).Id
                        |Error e ->  Feedback.giveFeedback true <| "Hiba a felhasználó lista lekérésekor: " + e
                |Error e -> Feedback.giveFeedback true <| "Hiba a képlista lekérésekor: " + e
        } |> Async.Start
    let RenderPage() =
        updateLists()
        SiteParts.ImageViewingTemplate()
            .OnSelection(
                fun _ -> updateLists()
            )
            .MemberList(
                Doc.BindSeqCached(
                    fun (u:Member) ->
                        SiteParts.ImageViewingTemplate.MemberItem()
                            .MemberID(string u.Id)
                            .MemberName(u.Name)
                            .Doc()
                ) userList.View
            )
            .SelectedMember(selectedMember.Lens (fun m -> string m) (fun m mi -> int mi))
            .ImageList(
                Doc.BindSeqCached (
                    fun (fn,uploader,(date:System.DateTime)) ->
                        SiteParts.ImageViewingTemplate.ImageItem()
                            .ImageLink("/img/" + fn)
                            .UploadDate(string date.Year + "-" + sprintf "%02d" date.Month + "-" + sprintf "%02d" date.Day + " " + sprintf "%02d" date.Hour + ":" + sprintf "%02d" date.Minute)
                            .Delete(
                                fun e ->
                                    ActionDispatcher.RunAction ImageUpload.DeleteImage (sessionID,fn) (Some updateLists)
                            )
                            .Doc()
                ) currentImages
            )
            .Doc()
    

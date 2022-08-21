namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module ServiceFeeAdmin =
    let UserList = ListModel.Create (fun (u:Member) -> u) []
    let PendingList = ListModel.Create (fun (f:PendingFee) -> f) []
    let SelectedUserID = Var.Create -1
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
    let updateUserList() =
        async{
            let! list = UserOperations.getUserList sessionID false false
            match list with
                |Ok l ->
                    UserList.Set l
                    SelectedUserID.Set (Seq.item 0 UserList.Value).Id
                |Error e ->
                    Feedback.giveFeedback true <| "Hiba a felhasználók lekérdezésekor: " + e
        } |> Async.Start
    let updatePendingList() =
        async{
            let! list = ServiceFee.getPendingFees sessionID 
            match list with
                |Ok l ->
                    PendingList.Set l
                    SelectedUserID.Set (Seq.item 0 UserList.Value).Id
                |Error e ->
                    Feedback.giveFeedback true <| "Hiba a szervizdíjak lekérdezésekor: " + e
        } |> Async.Start
    let RenderPage() =
        updateUserList()
        updatePendingList()
        SiteParts.ServiceTemplate()
            .MemberList(
                UserList.View |> Doc.BindSeqCached(
                    fun u ->
                        SiteParts.ServiceTemplate.MemberItem()
                            .UserID(string u.Id)
                            .Member(u.Name)
                            .Doc()
                )
            )
            .MemberSelectID(SelectedUserID.Lens (fun id -> string id) (fun id s -> int s))
            .ServiceList(
                PendingList.View |> Doc.BindSeqCached(
                    fun p ->
                        SiteParts.ServiceTemplate.ServiceListItem()
                            .ID(string p.ID)
                            .Name(p.Username)
                            .Paid(
                                fun e ->
                                    ActionDispatcher.RunAction ServiceFee.payFee (sessionID, p.ID) (Some updatePendingList)
                            )
                            .ServiceFee((string p.Amount) + "$")
                            .Doc()
                )
            )
            .Submit(
                fun e ->
                        if JavaScript.JS.IsNaN e.Vars.Fee.Value then Feedback.giveFeedback true "Kérlek számot adj meg árnak!"
                        else
                            ActionDispatcher.RunAction ServiceFee.submitPendingFee (sessionID,(int e.Vars.MemberSelectID.Value), (int e.Vars.Fee.Value)) (Some updatePendingList)
            )
            .Doc()

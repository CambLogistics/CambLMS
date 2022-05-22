namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module ServiceFeeAdmin =
    let UserList = ListModel.FromSeq [{Id= -1;Name="Senki";AccountID = -1;Email="@";Role = -1}]
    let PendingList = ListModel.FromSeq [{ID = -1;Amount = 0;Username = ""}]
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
    let updateUserList() =
        async{
            let! list = UserCallable.doGetUserList sessionID false
            UserList.Set list
        } |> Async.Start
    let updatePendingList() =
        async{
            let! list = ServiceFee.doGetPendingFees sessionID 
            PendingList.Set list
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
            .ServiceList(
                PendingList.View |> Doc.BindSeqCached(
                    fun p ->
                        SiteParts.ServiceTemplate.ServiceListItem()
                            .ID(string p.ID)
                            .Name(p.Username)
                            .Paid(
                                fun e ->
                                    async{
                                        let! result = ServiceFee.doPayFee sessionID p.ID
                                        updatePendingList result
                                    } |> Async.Start
                            )
                            .ServiceFee((string p.Amount) + "$")
                            .Doc()
                )
            )
            .Submit(
                fun e ->
                    async{
                        if JavaScript.JS.IsNaN e.Vars.Fee.Value then JavaScript.JS.Alert "Kérlek számot adj meg árnak!"
                        else
                            let! result = ServiceFee.doSubmitFee sessionID (int e.Vars.MemberSelectID.Value) (int e.Vars.Fee.Value)
                            updatePendingList result
                    } |> Async.Start
            )
            .Doc()
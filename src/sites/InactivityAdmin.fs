namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module InactivityAdmin =
    let requestList = ListModel.FromSeq [{UserName = "Loa_Ding";UserID=666;From=System.DateTime.Now;To=System.DateTime.Now;Reason="Nothing"}]
    let userList = ListModel.FromSeq [{UserName = "Loa_Ding";UserID=666;Status=false}]
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
    let updateList() =
        async{
            requestList.Clear()
            userList.Clear()
            let! newRequestList = Inactivity.getPendingRequests sessionID
            let! newUserList = Inactivity.getUserStatusList sessionID
            userList.Set newUserList
            requestList.Set newRequestList
        } |> Async.Start
    let RenderPage() =
        requestList.Clear()
        userList.Clear()
        updateList()
        SiteParts.InactivityAdminTemplate()
            .UserList(userList.View |> Doc.BindSeqCached (
                    fun user ->
                        SiteParts.InactivityAdminTemplate.UserListItem()
                            .Name(user.UserName)
                            .OnHoliday(if user.Status then "Nem" else "Igen")
                            .Doc()
                )
            )
            .InactivityList(requestList.View |> Doc.BindSeqCached (
                    fun request ->
                        SiteParts.InactivityAdminTemplate.InactivityListItem()
                            .Name(request.UserName)
                            .From(sprintf "%d-%02d-%02d %02d:%02d" request.From.Year request.From.Month request.From.Day request.From.Hour request.From.Minute)
                            .To(sprintf "%d-%02d-%02d %02d:%02d" request.To.Year request.To.Month request.To.Day request.To.Hour request.To.Minute)
                            .Approve(
                                fun _ ->
                                    async{
                                        let! result = Inactivity.decideRequest sessionID request true
                                        updateList result
                                    } |> Async.Start
                            )
                            .Deny(
                                fun _ ->
                                    async{
                                        let! result = Inactivity.decideRequest sessionID request false
                                        updateList result
                                    } |> Async.Start
                            )
                            .Doc()
                )
            )
            .Doc()

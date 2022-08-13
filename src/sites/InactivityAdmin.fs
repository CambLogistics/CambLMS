namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module InactivityAdmin =
    let requestList = ListModel.Create (fun (r:InactivityRequest) -> r) []
    let userList = ListModel.Create (fun (u:UserInactivityStatus) -> u) []
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
                            .Reason(request.Reason)
                            .Approve(
                                fun _ ->
                                    ActionDispatcher.RunAction Inactivity.decideRequest (sessionID, request, true) (Some updateList)
                            )
                            .Deny(
                                fun _ ->
                                    ActionDispatcher.RunAction Inactivity.decideRequest (sessionID, request, false) (Some updateList)
                            )
                            .Doc()
                )
            )
            .Doc()

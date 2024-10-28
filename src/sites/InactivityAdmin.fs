namespace camblms.sites

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

open camblms.dto
open camblms.templating
open camblms.server.controller

[<JavaScript>]
module InactivityAdmin =
    let requestList = ListModel.Create (fun (r: InactivityRequest) -> r) []
    let userList = ListModel.Create (fun (u: UserInactivityStatus) -> u) []
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value

    let updateList () =
        async {
            requestList.Clear()
            userList.Clear()
            let! newRequestList = InactivityController.getPendingRequests sessionID

            match newRequestList with
            | Ok rl -> rl |> requestList.Set
            | Error e -> Feedback.giveFeedback true <| "Hiba a kérvények lekérésekor: " + e

            let! newUserList = InactivityController.getUserStatusList sessionID

            match newUserList with
            | Ok ul -> ul |> userList.Set
            | Error e -> Feedback.giveFeedback true <| "Hiba a felhasználók lekérésekor: " + e
        }
        |> Async.Start

    let RenderPage () =
        requestList.Clear()
        userList.Clear()
        updateList ()

        SiteParts
            .InactivityAdminTemplate()
            .UserList(
                userList.View
                |> Doc.BindSeqCached(fun user ->
                    SiteParts.InactivityAdminTemplate
                        .UserListItem()
                        .Name(user.UserName)
                        .OnHoliday(if user.Status then "Nem" else "Igen")
                        .Doc())
            )
            .InactivityList(
                requestList.View
                |> Doc.BindSeqCached(fun request ->
                    SiteParts.InactivityAdminTemplate
                        .InactivityListItem()
                        .Name(request.UserName)
                        .From(
                            sprintf
                                "%d-%02d-%02d %02d:%02d"
                                request.From.Year
                                request.From.Month
                                request.From.Day
                                request.From.Hour
                                request.From.Minute
                        )
                        .To(
                            sprintf
                                "%d-%02d-%02d %02d:%02d"
                                request.To.Year
                                request.To.Month
                                request.To.Day
                                request.To.Hour
                                request.To.Minute
                        )
                        .Reason(request.Reason)
                        .Approve(fun _ ->
                            ActionDispatcher.RunAction
                                InactivityController.decideRequest
                                (sessionID, request, true)
                                (Some updateList))
                        .Deny(fun _ ->
                            ActionDispatcher.RunAction
                                InactivityController.decideRequest
                                (sessionID, request, false)
                                (Some updateList))
                        .Doc())
            )
            .Doc()

namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module NameChangeAdmin =
    let userList= ListModel.Create (fun (u:PendingChange) -> u) []
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
    let updateList() =
        async{
            let! pendingUsers = NameChangeServer.getPendingChanges sessionID
            userList.Set pendingUsers
        } |> Async.Start
    let RenderPage() =
        updateList()
        SiteParts.NameAdminTemplate()
            .UserList(
                userList.View |> Doc.BindSeqCached (
                    fun item ->
                        SiteParts.NameAdminTemplate.NameChangeUser()
                            .ID(string item.UserID)
                            .OldName(item.OldName)
                            .NewName(item.NewName)
                            .Approve(
                                fun e ->
                                    ActionDispatcher.RunAction NameChangeServer.decideNameChange (sessionID, item.UserID, true) (Some updateList)
                            )
                            .Deny(
                                fun e ->
                                    ActionDispatcher.RunAction NameChangeServer.decideNameChange (sessionID, item.UserID, false) (Some updateList)
                            )
                            .Doc()
                )
            )
            .Doc()

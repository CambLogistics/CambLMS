namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module NameChangeAdmin =
    let userList= ListModel.FromSeq [{UserID = -1;OldName = "Whatever";NewName = "WhoCares"}]
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
    let updateList =
        async{
            let! pendingUsers = NameChangeServer.doGetPendingChanges sessionID
            userList.Set pendingUsers
        } |> Async.Start
    let RenderPage =
        updateList
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
                                    NameChangeServer.doDecideNameChange sessionID item.UserID true |> Async.Start
                                    updateList
                            )
                            .Deny(
                                fun e ->
                                    NameChangeServer.doDecideNameChange sessionID item.UserID false |> Async.Start
                                    updateList
                            )
                            .Doc()
                )
            )
            .Doc()
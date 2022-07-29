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
            let! pendingUsers = NameChangeServer.doGetPendingChanges sessionID
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
                                    async{
                                        let! result = NameChangeServer.doDecideNameChange sessionID item.UserID true
                                        return updateList result
                                    } |> Async.Start
                            )
                            .Deny(
                                fun e ->
                                    async{
                                        let! result = NameChangeServer.doDecideNameChange sessionID item.UserID false 
                                        return updateList result
                                    } |> Async.Start
                            )
                            .Doc()
                )
            )
            .Doc()

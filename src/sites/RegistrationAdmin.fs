namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.JavaScript
open WebSharper.UI.Templating
open WebSharper.UI.Client

[<JavaScript>]
module RegistrationAdminPage =
    let pendingUsers= ListModel.Create (fun (u:Member) -> u) []
    let NameChangeList= ListModel.Create (fun (u:PendingChange) -> u) []
    let sessionID = (JavaScript.Cookies.Get "clms_sid").Value
    let updatePendingUserList() =
        async{
            let! userList = UserOperations.getUserList (JavaScript.Cookies.Get "clms_sid").Value true false
            match userList with
                |Ok l -> pendingUsers.Set l
                |Error e -> 
                    Feedback.giveFeedback true <| "Hiba a kérvények lekérdezésekor: " + e
        } |> Async.Start
    let updateNameChangeList() =
        async{
            let! pendingUsers = NameChangeServer.getPendingChanges sessionID
            match pendingUsers with
                |Ok l -> NameChangeList.Set l
                |Error e -> 
                    Feedback.giveFeedback true <| "Hiba a névváltoztatási kérvények lekérdezésekor: " + e
        } |> Async.Start
    let RenderPage() =
        updatePendingUserList()
        updateNameChangeList()
        SiteParts.RegistrationAdminTemplate()
            .PendingUserList(
                pendingUsers.View |> Doc.BindSeqCached (
                    fun pu ->
                        SiteParts.RegistrationAdminTemplate.PendingUserListItem()
                            .Name(pu.Name)
                            .AccID(string pu.AccountID)
                            .Email(pu.Email)
                            .UserID(string pu.Id)
                            .Approve(
                                fun e ->
                                    ActionDispatcher.RunAction UserOperations.approveUser (sessionID, pu.Id) (Some updatePendingUserList)
                            )
                            .Deny(
                                fun e ->
                                    ActionDispatcher.RunAction UserOperations.deleteUser (sessionID, pu.Id) (Some updatePendingUserList)
                            )
                            .Doc()
                )
            )
            .NameChangeUserList(
                NameChangeList.View |> Doc.BindSeqCached (
                    fun item ->
                        SiteParts.RegistrationAdminTemplate.NameChangeUser()
                            .ID(string item.UserID)
                            .OldName(item.OldName)
                            .NewName(item.NewName)
                            .Approve(
                                fun e ->
                                    ActionDispatcher.RunAction NameChangeServer.decideNameChange (sessionID, item.UserID, true) (Some updateNameChangeList)
                            )
                            .Deny(
                                fun e ->
                                    ActionDispatcher.RunAction NameChangeServer.decideNameChange (sessionID, item.UserID, false) (Some updateNameChangeList)
                            )
                            .Doc()
                )
            )
            .Doc()
    

namespace camblms

open WebSharper.UI.Html
open WebSharper
open WebSharper.UI
open WebSharper.JavaScript
open WebSharper.UI.Templating
open WebSharper.UI.Client



[<JavaScript>]
module RegistrationAdminPage =
    let pendingUsers= ListModel.Create (fun (u:Member) -> u) []
    let updateList() =
        async{
            let! userList = UserOperations.getUserList (JavaScript.Cookies.Get "clms_sid").Value true false
            match userList with
                |Ok l -> pendingUsers.Set l
                |Error e -> 
                    Feedback.giveFeedback true <| "Hiba a kérvények lekérdezésekor: " + e
        } |> Async.Start
    let RenderPage() =
        updateList()
        SiteParts.RegistrationAdminTemplate()
            .UserList(
                pendingUsers.View |> Doc.BindSeqCached (
                    fun pu ->
                        SiteParts.RegistrationAdminTemplate.UserListItem()
                            .Name(pu.Name)
                            .AccID(string pu.AccountID)
                            .Email(pu.Email)
                            .UserID(string pu.Id)
                            .Approve(
                                fun e ->
                                    ActionDispatcher.RunAction UserOperations.approveUser ((JavaScript.Cookies.Get "clms_sid").Value, pu.Id) (Some updateList)
                            )
                            .Deny(
                                fun e ->
                                    ActionDispatcher.RunAction UserOperations.deleteUser ((JavaScript.Cookies.Get "clms_sid").Value, pu.Id) (Some updateList)
                            )
                            .Doc()
                )
            )
            .Doc()
    

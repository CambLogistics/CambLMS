namespace camblms

open WebSharper.UI.Html
open WebSharper
open WebSharper.UI
open WebSharper.JavaScript
open WebSharper.UI.Templating
open WebSharper.UI.Client



[<JavaScript>]
module RegistrationAdminPage =
    let pendingUsers = ListModel.FromSeq [{Id = -1;Name="Dr.Who";AccountID=66666;Email="whoisthis@nope.no";Role = -1}]
    let updateList() =
        async{
            let! userList = UserOperations.doGetUserList (JavaScript.Cookies.Get "clms_sid").Value true false
            pendingUsers.Set userList
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
                                    async{
                                        let! result = UserOperations.doApproveUser (JavaScript.Cookies.Get "clms_sid").Value pu.Id
                                        return updateList result
                                    } |> Async.Start
                            )
                            .Deny(
                                fun e ->
                                    async{
                                        let! result = UserOperations.doDeleteUser (JavaScript.Cookies.Get "clms_sid").Value pu.Id 
                                        return updateList result
                                    } |> Async.Start
                            )
                            .Doc()
                )
            )
            .Doc()
    

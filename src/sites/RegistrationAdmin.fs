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
    let updateList =
        async{
            let! userList = UserCallable.doGetUserList (JavaScript.Cookies.Get "camblms_sid").Value  true
            pendingUsers.Set userList
        } |> Async.Start
    let RenderPage =
        updateList
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
                                    UserCallable.doApproveUser (JavaScript.Cookies.Get "camblms_sid").Value pu.Id
                                        |> Async.Start
                                    updateList
                            )
                            .Deny(
                                fun e ->
                                    UserCallable.doDeleteUser (JavaScript.Cookies.Get "camblms_sid").Value pu.Id 
                                        |> Async.Start
                                    updateList
                            )
                            .Doc()
                )
            )
            .Doc()
    

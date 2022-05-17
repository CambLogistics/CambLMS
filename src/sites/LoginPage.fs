namespace camblms

open WebSharper
open WebSharper.JavaScript

[<JavaScript>]
module LoginPage =
    let RenderPage =
        SiteTemplates.LoginTemplate()
            .RememberMe(
                match JavaScript.Cookies.Get "clms_rm" |> Optional.toOption with
                    |None -> false
                    |Some s -> s = "true"
            )
            .Name(
                match JavaScript.Cookies.Get "clms_rm_name" |> Optional.toOption with
                    |None -> ""
                    |Some s -> s
            )
            .Login(
                fun e ->
                    async{
                        let! result = UserCallable.doLogin e.Vars.Name.Value e.Vars.Password.Value
                        match result with
                        //TODO: display an error message when the template has a hole for that
                            |LoginResult.Success(id,exp) -> 
                                JS.Document.Cookie <- "clms_sid=" + id + ";expires=" + exp.ToString()
                                if e.Vars.RememberMe.Value then
                                    JavaScript.Cookies.Set("clms_rm","true")
                                    JavaScript.Cookies.Set("clms_rm_name",e.Vars.Name.Value)
                                else
                                    JavaScript.Cookies.Expire("clms_rm_name")
                                    JavaScript.Cookies.Set("clms_rm","false") 
                                JS.Window.Location.Replace "/"
                            |LoginResult.CredentialError -> ()
                            |LoginResult.DatabaseError -> ()
                            |LoginResult.NotApproved -> ()
                    } |> Async.Start
            )
            .Doc()
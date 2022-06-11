namespace camblms

open WebSharper
open WebSharper.JavaScript

[<JavaScript>]
module LoginPage =
    let RenderPage() =
        if JS.Window.Location.Href.Contains "registered" then Feedback.giveFeedback false "Regisztrációd sikeres és hamarosan jóváhagyásra kerül!"
        SiteParts.LoginPage()
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
                            |LoginResult.Success(id,exp) -> 
                                JS.Document.Cookie <- "clms_sid=" + id + ";expires=" + exp.ToString()
                                if e.Vars.RememberMe.Value then
                                    JavaScript.Cookies.Set("clms_rm","true")
                                    JavaScript.Cookies.Set("clms_rm_name",e.Vars.Name.Value)
                                else
                                    JavaScript.Cookies.Expire("clms_rm_name")
                                    JavaScript.Cookies.Set("clms_rm","false") 
                                JS.Window.Location.Replace "/"
                            |LoginResult.CredentialError ->
                                Feedback.giveFeedback true "Hibás felhasználónév vagy jelszó!"
                            |LoginResult.DatabaseError ->
                                Feedback.giveFeedback true "Adatbázishiba. Értesítsd a (műszaki) igazgatót!"
                            |LoginResult.NotApproved ->
                                Feedback.giveFeedback true "Regisztrációd még nem lett jóváhagyva. Kérjük légy türelemmel!"
                    } |> Async.Start
            )
            .Doc()
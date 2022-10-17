namespace camblms

open WebSharper
open WebSharper.JavaScript

[<JavaScript>]
module LoginPage =
    let callLogin name password =
        async {
            let! result = User.loginUser(name,password)
            match result with
            | LoginResult.Success id ->
                JS.Document.Cookie <- "clms_sid=" + id
                JS.Window.Location.Replace "/"
            | LoginResult.CredentialError -> Feedback.giveFeedback true "Hibás felhasználónév vagy jelszó!"
            | LoginResult.DatabaseError -> Feedback.giveFeedback true "Adatbázishiba. Értesítsd a (műszaki) igazgatót!"
            | LoginResult.NotApproved ->
                Feedback.giveFeedback true "Regisztrációd még nem lett jóváhagyva. Kérjük légy türelemmel!"
        }
    let RenderPage () =
        if JS.Window.Location.Href.Contains "registered" then
            Feedback.giveFeedback false "Regisztrációd sikeres és hamarosan jóváhagyásra kerül!"
        SiteParts
            .LoginPage()
            .Login(fun e ->
                callLogin e.Vars.Name.Value e.Vars.Password.Value
                |> Async.Start)
            .Kblogin(
                fun e ->
                    if e.Event.KeyCode = 13 then callLogin e.Vars.Name.Value e.Vars.Password.Value |> Async.Start
            )
            .Doc()

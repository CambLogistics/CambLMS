namespace camblms.sites

open WebSharper
open WebSharper.JavaScript

open camblms.dto
open camblms.templating
open camblms.server.controller

[<JavaScript>]
module LoginPage =
    let callLogin name password =
        async {
            let! result = UserController.loginUser (name, password)

            match result with
            | LoginResult.Success id ->
                JS.Document.Cookie <- "clms_sid=" + id
                JS.Window.Location.Replace "/"
            | LoginResult.CredentialError -> Feedback.giveFeedback true "Hibás felhasználónév vagy jelszó!"
            | LoginResult.DatabaseError -> Feedback.giveFeedback true "Adatbázishiba. Értesítsd a (műszaki) igazgatót!"
            | LoginResult.NotApproved ->
                Feedback.giveFeedback true "Regisztrációd még nem lett jóváhagyva. Kérjük légy türelemmel!"
        }

    let register name password accid email =
        async {
            if not (JS.IsNaN accid) then
                let! result = UserController.registerUser (name, password, (int accid), email)

                match result with
                | RegisterResult.Success -> JS.Window.Location.Replace "/login?registered=true"
                | Exists -> Feedback.giveFeedback true "Ilyen felhasználó már létezik!"
                | RegisterResult.DatabaseError ->
                    Feedback.giveFeedback true "Adatbázishiba. Kérjük értesítsd a (műszaki) igazgatót!"
                | MissingData -> Feedback.giveFeedback true "Ellenőrizd a bevitt adatokat!"
                | BadName -> Feedback.giveFeedback true "Nem megfelelő név!"
                | BadPassword -> Feedback.giveFeedback true "Nem megfelelő jelszó!"
            else
                Feedback.giveFeedback true "Ellenőrizd a bevitt adatokat!"
        }

    let RenderPage () =
        if JS.Window.Location.Href.Contains "registered" then
            Feedback.giveFeedback false "Regisztrációd sikeres és hamarosan jóváhagyásra kerül!"

        SiteParts
            .LoginPage()
            .Login(fun e -> callLogin e.Vars.Name.Value e.Vars.Password.Value |> Async.Start)
            .Kblogin(fun e ->
                if e.Event.KeyCode = 13 then
                    callLogin e.Vars.Name.Value e.Vars.Password.Value |> Async.Start)
            .Kbregister(fun e ->
                if e.Event.KeyCode = 13 then
                    register e.Vars.Name.Value e.Vars.Password.Value e.Vars.AccID.Value e.Vars.Email.Value
                    |> Async.Start)
            .Register(fun e ->
                register e.Vars.Name.Value e.Vars.Password.Value e.Vars.AccID.Value e.Vars.Email.Value
                |> Async.Start)
            .Doc()

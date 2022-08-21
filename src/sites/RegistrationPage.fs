namespace camblms

open WebSharper
open WebSharper.JavaScript

[<JavaScript>]
module RegistrationPage =
    let register name password passwordrepeat accid email =
        async {
            if password = passwordrepeat && not (JS.IsNaN accid) then
                let! result = User.registerUser(name,password,(int accid),email)
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
        SiteParts
            .RegistrationPage()
            .Kbregister(fun e ->
                if e.Event.KeyCode = 13 then
                    register
                        e.Vars.Name.Value
                        e.Vars.Password.Value
                        e.Vars.PasswordRepeat.Value
                        e.Vars.AccID.Value
                        e.Vars.Email.Value
                    |> Async.Start)
            .Register(fun e ->
                register
                    e.Vars.Name.Value
                    e.Vars.Password.Value
                    e.Vars.PasswordRepeat.Value
                    e.Vars.AccID.Value
                    e.Vars.Email.Value
                |> Async.Start)
            .Doc()

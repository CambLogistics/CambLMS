namespace camblms

open WebSharper
open WebSharper.JavaScript

[<JavaScript>]
module RegistrationPage =
    let RenderPage =
        SiteTemplates.RegistrationTemplate()
            .Register(
                fun e ->
                    async{
                        if e.Vars.Password.Value = e.Vars.PasswordRepeat.Value && not (JS.IsNaN e.Vars.AccID.Value)
                            then
                                let! result = 
                                    UserCallable.doRegister e.Vars.Name.Value e.Vars.Password.Value (int e.Vars.AccID.Value) e.Vars.Email.Value
                                match result with
                                    |RegisterResult.Success ->
                                        Feedback.giveFeedback false "Regisztrációd sikeres és hamarosan jóváhagyásra kerül!"
                                    |Exists ->
                                        Feedback.giveFeedback true "Ilyen felhasználó már létezik!"
                                    |RegisterResult.DatabaseError ->
                                        Feedback.giveFeedback true "Adatbázishiba. Kérjük értesítsd a (műszaki) igazgatót!"
                                    |MissingData ->
                                        Feedback.giveFeedback true "Ellenőrizd a bevitt adatokat!"
                                    |BadName ->
                                        Feedback.giveFeedback true "Nem megfelelő név!"
                                    |BadPassword ->
                                        Feedback.giveFeedback true "Nem megfelelő jelszó!"
                        else Feedback.giveFeedback true "Ellenőrizd a bevitt adatokat!"
                    } |> Async.Start
            )
            .Doc()

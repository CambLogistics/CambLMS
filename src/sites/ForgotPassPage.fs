namespace camblms

module ForgotPassPage =
    let callForgotPassHandler email =
        async {
            let! result = ForgotPass.changePasswordByEmail email

            match result with
            | MailSent -> Feedback.giveFeedback false "A levél az új jelszóval elküldésre került!"
            | MailError -> Feedback.giveFeedback true "Hiba a levelezőrendszerben! Értesítsd a (műszaki) igazgatót!"
            | ForgotPassStatus.DatabaseError ->
                Feedback.giveFeedback true "Adatbázishiba! Értesítsd a (műszaki) igazgatót!"
            | NoSuchUser -> Feedback.giveFeedback true "Nincs ezen e-mail címmel regisztrált felhasználó!"
        }
        |> Async.Start

    let RenderPage () =
        SiteParts
            .ForgotPassTemplate()
            .Submit(fun e -> callForgotPassHandler e.Vars.Email.Value)
            .Kbemail(fun e ->
                if e.Event.KeyCode = 13 then
                    callForgotPassHandler e.Vars.Email.Value)
            .Doc()

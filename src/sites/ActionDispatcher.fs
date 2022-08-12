namespace camblms

open WebSharper

[<JavaScript>]
module ActionDispatcher =
    let RunAction action arg =
        async{
            let! result = action arg
            match result with
                |ActionResult.Success -> return Feedback.giveFeedback false "Művelet sikeres!"
                |ActionResult.InvalidSession -> return Feedback.giveFeedback true "Érvénytelen munkamenet. Lépj be újra!"
                |ActionResult.InactiveUser -> return Feedback.giveFeedback true "Szabadság alatt nem teheted meg ezt!"
                |ActionResult.InsufficientPermissions -> return Feedback.giveFeedback true "Nincs jogosultságod ehhez a művelethez!"
                |ActionResult.DatabaseError -> return Feedback.giveFeedback true "Adatbázishiba. Értesítsd a (műszaki) igazgatót!"
                |ActionResult.OtherError e -> return Feedback.giveFeedback true e.Message
        } |> Async.Start
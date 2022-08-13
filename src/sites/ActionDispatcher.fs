namespace camblms

open WebSharper

[<JavaScript>]
module ActionDispatcher =
    let RunAction action arg postActionCallback =
        async{
            let! result = action arg
            match result with
                |ActionResult.Success -> 
                    Feedback.giveFeedback false "Művelet sikeresen befejezve!"
                    match postActionCallback with
                        |None -> ()
                        |Some f -> f()
                |ActionResult.InvalidSession -> return Feedback.giveFeedback true "Érvénytelen munkamenet. Lépj be újra!"
                |ActionResult.InactiveUser -> return Feedback.giveFeedback true "Szabadság alatt nem teheted meg ezt!"
                |ActionResult.InsufficientPermissions -> return Feedback.giveFeedback true "Nincs jogosultságod ehhez a művelethez!"
                |ActionResult.DatabaseError -> return Feedback.giveFeedback true "Adatbázishiba. Értesítsd a (műszaki) igazgatót!"
                |ActionResult.OtherError e -> return Feedback.giveFeedback true e
        } |> Async.Start
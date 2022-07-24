namespace camblms

open WebSharper

[<JavaScript>]
module InactivityPage =
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
    let RenderPage() =
        SiteParts.InactivityTemplate()
            .Submit(
                fun e ->
                    async{
                        let! result = Inactivity.requestInactivity sessionID e.Vars.Begin.Value e.Vars.End.Value e.Vars.Reason.Value
                        match result with
                            |InactivityRequestSuccess.Success -> Feedback.giveFeedback false "Kérelmed sikeresen leadásra került!"
                            |InactivityRequestSuccess.Overlap -> Feedback.giveFeedback true "A kérvényezett időszak ütközik egy másik inaktivitási időszakkal!"
                            |InactivityRequestSuccess.InvalidSession -> Feedback.giveFeedback true "Érvénytelen munkamenet. Jelentkezz be újra!"
                            |InactivityRequestSuccess.DatabaseError -> Feedback.giveFeedback true "Adatbázishiba. Értesítsd a (műszaki) igazgatót!"
                    } |> Async.Start
            )
            .Doc()
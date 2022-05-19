namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module NameChangePage =
    let RenderPage =
        SiteParts.NameChangeTemplate()
            .Submit(
                fun e ->
                    let oldName = e.Vars.OldName.Value
                    let newName = e.Vars.NewName.Value
                    let password = e.Vars.Password.Value
                    let sid = JavaScript.Cookies.Get("clms_sid").Value
                    if String.length oldName < 5 || String.length newName < 5 || String.length password < 3 then 
                        Feedback.giveFeedback true "Ellenőrizd a bevitt adatokat!"
                    else
                    async{
                        let! result = NameChangeServer.doProposeNameChange sid oldName newName password
                        match result with
                            |Success -> 
                                Feedback.giveFeedback false "Névváltoztatási kérelmed beadásra került"
                            |WrongPassword ->
                                Feedback.giveFeedback true "Rossz jelszó!"
                            |WrongOldName ->
                                Feedback.giveFeedback true "Rossz régi név!"
                            |WrongNewName ->
                                Feedback.giveFeedback true "Nem megfelelő régi név!"
                            |ChangeAlreadyPending ->
                                Feedback.giveFeedback true "Már van el nem bírált kérelmed!"
                            |InvalidSession ->
                                Feedback.giveFeedback true "Rossz munkamenet. Lépj ki és lépj be újra!"
                            |DatabaseError ->
                                Feedback.giveFeedback true "Adatbázishiba! Értesítsd a (műszaki) igazgatót!"
                    } |> Async.Start
            )
            .Doc()
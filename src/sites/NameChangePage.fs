namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module NameChangePage =
    let RenderPage() =
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
                        ActionDispatcher.RunAction NameChangeServer.proposeNameChange (sid, oldName, newName, password) None
            )
            .Doc()
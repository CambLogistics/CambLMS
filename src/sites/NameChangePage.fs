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
                    //TODO: display an error/success message when the template has a hole for that
                    if String.length oldName < 5 || String.length newName < 5 || String.length password < 3 then ()
                    else
                    async{
                        let! result = NameChangeServer.doProposeNameChange sid oldName newName password
                        match result with
                            |Success -> ()
                            |WrongPassword -> ()
                            |WrongOldName -> ()
                            |WrongNewName -> ()
                            |ChangeAlreadyPending -> ()
                            |InvalidSession -> ()
                            |DatabaseError -> ()
                    } |> Async.Start
            )
            .Doc()
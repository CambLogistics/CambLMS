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
                                //TODO: display a result message when the template has a hole for that
                                match result with
                                    |RegisterResult.Success -> ()
                                    |Exists -> ()
                                    |RegisterResult.DatabaseError -> ()
                                    |MissingData -> ()
                                    |BadName -> ()
                                    |BadPassword -> ()
                    } |> Async.Start
            )
            .Doc()

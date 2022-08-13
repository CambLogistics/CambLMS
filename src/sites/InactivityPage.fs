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
                        ActionDispatcher.RunAction Inactivity.requestInactivity (sessionID, e.Vars.Begin.Value, e.Vars.End.Value, e.Vars.Reason.Value) None
                        e.Vars.Begin.Set ""
                        e.Vars.End.Set ""
                        e.Vars.Reason.Set ""
                    } |> Async.Start
            )
            .Doc()
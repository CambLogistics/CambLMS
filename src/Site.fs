namespace camblms

open WebSharper
open WebSharper.Sitelets

open camblms.server.routing

module Site =

    [<Website>]
    let Main = Application.MultiPage(fun ctx endpoint -> Routing.MakeRoute ctx endpoint)

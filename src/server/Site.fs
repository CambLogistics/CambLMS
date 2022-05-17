namespace camblms

open WebSharper
open WebSharper.Sitelets
    
module Site =

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx endpoint ->
            Routing.MakeRoute ctx endpoint
        )

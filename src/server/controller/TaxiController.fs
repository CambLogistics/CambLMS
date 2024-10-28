namespace camblms.server.controller

open WebSharper

open camblms.dto
open camblms.server.service

module TaxiController =

    [<Rpc>]
    let doCalculatePrice source dest =
        async { return TaxiService.calculatePrice source dest }

    [<Rpc>]
    let submitCall (sid, source, dest) =
        async {
            try
                let user = UserService.getUserFromSID sid
                let price = TaxiService.calculatePrice source dest
                price |> CallsService.registerCall user.Value <| CallType.Taxi
                return ActionResult.Success
            with _ ->
                return ActionResult.DatabaseError
        }

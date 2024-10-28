namespace camblms.server.controller

open WebSharper

open camblms.dto
open camblms.server.service

module TowController =
    [<Rpc>]
    let doCalculatePrice source dest =
        async { return TowService.calculatePrice source dest }

    [<Rpc>]
    let submitCall (sid, source, dest) =
        async {
            try
                let user = UserService.getUserFromSID sid
                let price = TowService.calculatePrice source dest
                price |> CallsService.registerCall user.Value <| CallType.Taxi
                return ActionResult.Success
            with _ ->
                return ActionResult.DatabaseError
        }

    [<Rpc>]
    let doGetGarageList () =
        async { return! TowService.getGarageList () }

namespace camblms.server.controller

open WebSharper
open System

open camblms.dto
open camblms.server.service

module InactivityController =

    [<Rpc>]
    let getUserStatusList sessionID =
        async {
            try
                let hasPermission =
                    PermissionService.checkPermission sessionID Permissions.InactivityAdmin

                if not hasPermission then
                    return Error "Nem nézheted meg a felhasználói jogosultságokat!"
                else
                    return Ok <| InactivityService.getUserStatusList ()
            with e ->
                return Error e.Message
        }

    [<Rpc>]
    let requestInactivity (sessionID, start, ending, reason) =
        async {
            try
                let user = UserService.getUserFromSID sessionID

                match user with
                | None -> return ActionResult.InvalidSession
                | Some u ->
                    let startDate = DateTime.Parse(start)
                    let endDate = DateTime.Parse(ending)
                    let overlap = InactivityService.hasOverlappingRequest u startDate endDate

                    if overlap then
                        return ActionResult.OtherError "Erre az időszakra(vagy egy részére) már van beadott kérelmed!"
                    else
                        do InactivityService.requestInactivity u startDate endDate reason
                        return ActionResult.Success
            with _ ->
                return ActionResult.DatabaseError
        }

    [<Rpc>]
    let decideRequest (sessionID, (req: InactivityRequest), decision) =
        async {
            try
                let hasPermission =
                    PermissionService.checkPermission sessionID Permissions.InactivityAdmin

                if not hasPermission then
                    return InsufficientPermissions
                else
                    do InactivityService.decideRequest req decision
                    return ActionResult.Success
            with e ->
                return OtherError e.Message
        }

    [<Rpc>]
    let getPendingRequests sessionID =
        async {
            try
                let hasPermission =
                    PermissionService.checkPermission sessionID Permissions.InactivityAdmin

                if not hasPermission then
                    return Error "Nincs jogosultságod a szabadságkérelmek kezeléséhez!"
                else
                    return Ok <| InactivityService.getPendingRequests ()
            with e ->
                return Error e.Message
        }

namespace camblms.server.controller

open WebSharper

open camblms.dto
open camblms.server.service

module CallsController =
    [<Rpc>]
    let getAreaList () =
        async {
            try
                return Ok <| CallsService.getAreaList ()
            with e ->
                return Error e.Message
        }

    [<Rpc>]
    let rotateWeek sid =
        async {
            let hasPermission = PermissionService.checkPermission sid Permissions.CloseWeek

            if not hasPermission then
                return ActionResult.InsufficientPermissions
            else
                try
                    do CallsService.rotateWeek ()
                    return ActionResult.Success
                with _ ->
                    return ActionResult.DatabaseError
        }

    [<Rpc>]
    let rotateTopList sid =
        async {
            let hasPermission = PermissionService.checkPermission sid Permissions.CloseWeek

            if not hasPermission then
                return ActionResult.InsufficientPermissions
            else
                try
                    do CallsService.rotateTopList ()
                    return ActionResult.Success
                with _ ->
                    return ActionResult.DatabaseError
        }

    [<Rpc>]
    let getUserListWithCalls sid duration =
        async {
            try
                let hasPermission = PermissionService.checkPermission sid Permissions.Admin

                if not hasPermission then
                    return Error "Nincs jogosultságod a felhasználók hívásos listáját lekérni!"
                else
                    return Ok <| CallsService.getUserListWithCalls duration
            with e ->
                return Error e.Message
        }

    [<Rpc>]
    let getCallsOfUser sid =
        async {
            try
                return Some <| CallsService.getCallsOfUser (UserService.getUserFromSID sid).Value
            with _ ->
                return None
        }

    [<Rpc>]
    let getWeeklyCallPercentage sid =
        async {
            try
                let user = UserService.getUserFromSID sid

                match user with
                | Some u ->
                    let weeklyCallPercentage = CallsService.getWeeklyCallPercentage u
                    return Ok weeklyCallPercentage
                | None -> return Error "Érvénytelen session!"
            with e ->
                return Error e.Message
        }

    [<Rpc>]
    let clientGetDPStatus () =
        async { return CallsService.isDoublePrice () }

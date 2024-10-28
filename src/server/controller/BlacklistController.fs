namespace camblms.server.controller

open WebSharper

open camblms.dto
open camblms.server.service

module BlacklistController =
    [<Rpc>]
    let getBlackListItems sid =
        async {
            try
                let hasPermission = PermissionService.checkPermission sid Permissions.MemberAdmin

                if not hasPermission then
                    return Error "Nincs jogosultságod a felhasználók kezeléséhez!"
                else
                    return Ok <| BlacklistService.getBlacklist ()
            with e ->
                return Error e.Message
        }

    [<Rpc>]
    let setBlackListItem (sid, bli) =
        async {
            try
                let hasPermission = PermissionService.checkPermission sid Permissions.MemberAdmin

                if not hasPermission then
                    return ActionResult.InsufficientPermissions
                else
                    do BlacklistService.setBlacklistItem bli
                    return ActionResult.Success

            with e ->
                return ActionResult.DatabaseError
        }

    [<Rpc>]
    let deleteItem (sid, bli) =
        async {
            try
                let hasPermission = PermissionService.checkPermission sid Permissions.MemberAdmin

                if not hasPermission then
                    return ActionResult.InsufficientPermissions
                else
                    do BlacklistService.deleteBlacklistItem bli
                    return ActionResult.Success
            with e ->
                return ActionResult.DatabaseError
        }

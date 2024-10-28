namespace camblms.server.controller

open WebSharper

open camblms.dto
open camblms.server.service

module ServiceFeeController =
    [<Rpc>]
    let getPendingFees sid =
        async {
            let hasPermission =
                PermissionService.checkPermission sid Permissions.ServiceFeeAdmin

            if not hasPermission then
                return Error "Nincs jogosultságod a szervizdíjak kezeléséhez!"
            else
                try
                    return Ok <| ServiceFeeService.getPendingFees ()
                with e ->
                    return Error e.Message
        }

    [<Rpc>]
    let submitPendingFee (sid, userid, amount) =
        async {
            let hasPermission =
                PermissionService.checkPermission sid Permissions.ServiceFeeAdmin

            if not hasPermission then
                return InsufficientPermissions
            else
                try
                    do ServiceFeeService.submitPendingFee userid amount
                    return ActionResult.Success
                with e ->
                    return OtherError e.Message
        }

    [<Rpc>]
    let payFee (sid, feeID) =
        async {
            let hasPermission =
                PermissionService.checkPermission sid Permissions.ServiceFeeAdmin

            if not hasPermission then
                return InsufficientPermissions
            else
                try
                    do ServiceFeeService.payFee feeID
                    return ActionResult.Success
                with e ->
                    return OtherError e.Message
        }

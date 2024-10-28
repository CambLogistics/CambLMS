namespace camblms.server.controller

open WebSharper

open camblms.dto
open camblms.server.service

module NameChangeController =
    [<Rpc>]
    let proposeNameChange (sid, oldname, newname, password) =
        async {
            let user = UserService.getUserFromSID sid
            let passwordOkay = UserService.authenticateLoggedInUser sid password

            match user with
            | None -> return ActionResult.InvalidSession
            | Some u ->
                if u.Name = oldname |> not then
                    return OtherError "Rosszul adtad meg a régi neved!"
                else if not passwordOkay then
                    return OtherError "Rossz jelszó!"
                else if String.length newname < 5 then
                    return OtherError "Az új név nem felel meg a követelményeknek!"
                else
                    try
                        let hasPendingRequest = NameChangeService.hasPendingRequest u

                        if hasPendingRequest then
                            return OtherError "Már van elbírálásra váró kérelmed!"
                        else
                            NameChangeService.requestNameChange u newname
                            return ActionResult.Success
                    with _ ->
                        return ActionResult.DatabaseError
        }

    [<Rpc>]
    let decideNameChange (sid, userID, decision) =
        async {
            let hasPermission = PermissionService.checkPermission sid Permissions.MemberAdmin

            if not hasPermission then
                return InsufficientPermissions
            else
                try
                    do NameChangeService.decideRequest userID decision
                    return ActionResult.Success
                with e ->
                    return OtherError e.Message
        }

    [<Rpc>]
    let getPendingChanges sid =
        async {
            let hasPermission = PermissionService.checkPermission sid Permissions.MemberAdmin

            if not hasPermission then
                return Error "Nincs jogosultságod elbírálni a névváltoztatási kérelmeket!"
            else
                try
                    return Ok <| NameChangeService.getPendingChanges ()
                with e ->
                    return Error e.Message
        }

namespace camblms.server.service

open WebSharper

open camblms.dto
open camblms.server.database

module PermissionService =
    let getUserPermissions (user: Member) =
        try
            let db = Database.getDataContext ()

            let rolePermissions =
                (query {
                    for p in db.Camblogistics.permissions do
                        where (p.RoleId = user.Role.Level)
                        exactlyOne
                })
                    .Permissions

            let userOverride =
                let upo =
                    (query {
                        for po in db.Camblogistics.permissionoverrides do
                            where (po.UserId = user.Id)
                            select po.Permissions
                    })

                if Seq.isEmpty upo then 0u else Seq.item 0 upo

            rolePermissions ||| userOverride
        with _ ->
            0u

    let checkPermission sid (perm: Permissions) =
        let user = UserService.getUserFromSID sid

        match user with
        | None -> false
        | Some u -> ((getUserPermissions u &&& (LanguagePrimitives.EnumToValue perm)) > 0u)

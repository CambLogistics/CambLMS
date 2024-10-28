namespace camblms.server.controller

open WebSharper

open camblms.dto
open camblms.server.service

type LoginStatus =
    | LoggedIn of Member
    | LoggedOut

module UserController =
    open System.Net.Mail

    [<Rpc>]
    let loginUser (name, password) =
        async {
            try
                return UserService.loginUser name password
            with _ ->
                return LoginResult.DatabaseError
        }

    [<Rpc>]
    let registerUser (name, password, accountid, email) =
        async {
            if String.length password < 5 || String.length name < 3 || String.length email < 5 then
                return MissingData
            else
                try
                    return UserService.registerUser (name, password, accountid, email)
                with _ ->
                    return RegisterResult.DatabaseError
        }

    [<Rpc>]
    let getRankList () =
        async {
            try
                let ranks = UserService.getRankList ()
                return Ok(Seq.toList ranks)
            with e ->
                return Error e.Message
        }

    [<Rpc>]
    let getUserProfile sid =
        async {
            try
                let user = UserService.getUserFromSID sid
                return user
            with _ ->
                return None
        }

    [<Rpc>]
    let changeUserPassword (sid, oldPassword, newPassword) =
        async {
            try
                if String.length newPassword < 3 then
                    return ActionResult.OtherError "Ez a jelszó nem megfelelő!"
                else
                    let validAuth = UserService.authenticateLoggedInUser sid oldPassword

                    if validAuth |> not then
                        return ActionResult.OtherError "A régi jelszót rosszul adtad meg!"
                    else
                        let user = UserService.getUserFromSID sid

                        match user with
                        | None -> return ActionResult.InvalidSession
                        | Some us ->
                            do UserService.changeUserPassword us newPassword
                            return ActionResult.Success
            with _ ->
                return ActionResult.DatabaseError
        }

    [<Rpc>]
    let deleteUser (sid, userid) =
        async {
            let hasPermission = PermissionService.checkPermission sid Permissions.MemberAdmin

            if not hasPermission then
                return ActionResult.InsufficientPermissions
            else
                try
                    do UserService.deleteUser userid
                    return ActionResult.Success
                with e ->
                    return OtherError e.Message
        }

    [<Rpc>]
    let approveUser (sid, userid) =
        async {
            let hasPermission = PermissionService.checkPermission sid Permissions.MemberAdmin

            if not hasPermission then
                return InsufficientPermissions
            else
                try
                    do UserService.approveUser userid
                    return ActionResult.Success
                with e ->
                    return OtherError e.Message
        }

    [<Rpc>]
    let getUserList sid pending showDeleted =
        async {
            try
                let hasPermission = PermissionService.checkPermission sid Permissions.Admin

                if not hasPermission then
                    return Error "Nincs jogosultságod a felhasználók listáját lekérni!"
                else
                    return Ok <| UserService.getUserList pending showDeleted
            with e ->
                return Error e.Message
        }

    [<Rpc>]
    let changeUserRank (sid, userID, newRank) =
        async {
            let hasPermission = PermissionService.checkPermission sid Permissions.MemberAdmin

            if not hasPermission then
                return InsufficientPermissions
            else
                let user = UserService.getUserFromSID sid

                match user with
                | None -> return InvalidSession
                | Some u ->
                    let target = UserService.getUserByID userID

                    if u.Role <= target.Value.Role || u.Id = userID then
                        return InsufficientPermissions
                    else
                        try
                            do UserService.changeUserRank userID newRank
                            return ActionResult.Success
                        with e ->
                            return OtherError e.Message
        }

    [<Rpc>]
    let changePasswordByEmail email =
        async {
            try
                let user = UserService.getUserByEmail email
                ForgotPassService.changePasswordByEmail user.Value
                return MailSent
            with
            | :? System.NullReferenceException -> return NoSuchUser
            | :? System.ArgumentNullException -> return NoSuchUser
            | :? SmtpException -> return MailError
            | _ -> return DatabaseError
        }

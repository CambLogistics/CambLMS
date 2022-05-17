namespace camblms

open WebSharper

[<JavaScript>]
module PasswordChangePage =
    let RenderPage =
        SiteParts.PasswordChangeTemplate()
            .ChangePassword(
                fun e ->
                    let sessionID = JavaScript.Cookies.Get("camblms_sid").Value
                    let oldPassword = e.Vars.OldPassword.Value
                    let newPassword = e.Vars.NewPassword.Value
                    let newPasswordRepeat = e.Vars.NewPasswordRepeat.Value
                    //TODO: display result message when the template has a hole for that
                    if not (newPassword = newPasswordRepeat) then ()
                    else
                    async{
                        let! result = UserCallable.doChangeUserPassword sessionID oldPassword newPassword
                        match result with
                            |PasswordChangeResult.Success -> ()
                            |PasswordChangeResult.WrongPassword -> ()
                            |PasswordChangeResult.BadNewPassword -> ()
                            |PasswordChangeResult.DatabaseError -> ()
                            |PasswordChangeResult.InvalidSession -> ()
                    } |> Async.Start
            )
            .Doc()
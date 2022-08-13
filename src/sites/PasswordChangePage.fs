namespace camblms

open WebSharper

[<JavaScript>]
module PasswordChangePage =
    let RenderPage() =
        SiteParts.PasswordChangeTemplate()
            .ChangePassword(
                fun e ->
                    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
                    let oldPassword = e.Vars.OldPassword.Value
                    let newPassword = e.Vars.NewPassword.Value
                    let newPasswordRepeat = e.Vars.NewPasswordRepeat.Value
                    if not (newPassword = newPasswordRepeat) then Feedback.giveFeedback true "Az új jelszó és annak ismétlése nem egyezik meg!"
                    else
                        ActionDispatcher.RunAction User.changeUserPassword (sessionID, oldPassword, newPassword) None
            )
            .Doc()
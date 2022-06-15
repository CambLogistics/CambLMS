namespace camblms

open WebSharper

[<JavaScript>]
module PasswordChangePage =
    let RenderPage() =
        SiteParts.PasswordChangeTemplate()
            .ChangePassword(
                fun e ->
                    let sessionID = JavaScript.Cookies.Get("camblms_sid").Value
                    let oldPassword = e.Vars.OldPassword.Value
                    let newPassword = e.Vars.NewPassword.Value
                    let newPasswordRepeat = e.Vars.NewPasswordRepeat.Value
                    if not (newPassword = newPasswordRepeat) then ()
                    else
                    async{
                        let! result = UserOperations.doChangeUserPassword sessionID oldPassword newPassword
                        match result with
                            |PasswordChangeResult.Success ->
                                Feedback.giveFeedback false "Jelszavad megváltoztatásra került!"
                            |PasswordChangeResult.WrongPassword ->
                                Feedback.giveFeedback true "Rossz jelszó!"
                            |PasswordChangeResult.BadNewPassword ->
                                Feedback.giveFeedback true "Új jelszavad nem felel meg a követelményeknek!"
                            |PasswordChangeResult.DatabaseError ->
                                Feedback.giveFeedback true "Adatbázishiba. Értesítsd a (műszaki) igazgatót!" 
                            |PasswordChangeResult.InvalidSession ->
                                Feedback.giveFeedback true "Érvénytelen munkamenet. Lépj ki és lépj be újra!"
                    } |> Async.Start
            )
            .Doc()
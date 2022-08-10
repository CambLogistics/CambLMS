namespace camblms

open WebSharper
open System.Net.Mail
open System.Text.RegularExpressions
open System.IO

[<JavaScript>]
type ForgotPassStatus =
    | MailSent
    | MailError
    | NoSuchUser
    | DatabaseError

module ForgotPass =
    let emailConfig = Config.readEmail ()

    let generateEmail (username: string) (password: string) =
        let mailTemplate =
            try
                File.ReadAllText("templates/email/forgotpass.html")
            with
            | _ ->
                @"<html>
                                        <head>
                                                <meta charset=""utf-8"">
                                        </head>
                                        <body>
                                                Tisztelt ${Name}!<br/><br/> 
                                                
                                                Az ön új jelszava: ${NewPassword}<br/><br/>
                                                
                                                Üdvözlettel:<br/>
                                                CambLogistics csapata
                                        </body>
                                        </html>"

        Regex.Replace(Regex.Replace(mailTemplate, @"\$\{Name\}", username), @"\$\{NewPassword\}", password)
    [<Rpc>]
    let changePasswordByEmail email =
        async {
            let db = Database.getDataContext ()

            let userSeq =
                query {
                    for u in db.Camblogistics.users do
                        where (u.Email = email && u.Accepted = (sbyte 1) && u.Deleted = (sbyte 0))
                        select u
                }

            match (Seq.length userSeq > 0) with
            | true ->
                try
                    let user = userSeq |> Seq.item 0
                    let newPassword = RandomString.getRandomString 12
                    let smtpClient = new SmtpClient(emailConfig.Host)
                    smtpClient.Port <- emailConfig.Port

                    let message =
                        new MailMessage(
                            new MailAddress("noreply@" + emailConfig.Domain, emailConfig.SenderName),
                            new MailAddress(email)
                        )

                    message.IsBodyHtml <- true
                    message.Body <- generateEmail user.Name newPassword
                    message.BodyEncoding <- System.Text.Encoding.UTF8
                    message.Subject <- "CambLogistics elfelejtett jelszó"
                    message.SubjectEncoding <- System.Text.Encoding.UTF8
                    smtpClient.Send message
                    user.Password <- newPassword |> User.hashPassword
                    db.SubmitUpdates()
                    message.Dispose()
                    smtpClient.Dispose()
                    return MailSent
                with
                | :? SmtpException -> return MailError
                | _ -> return DatabaseError
            | false -> return NoSuchUser
        }

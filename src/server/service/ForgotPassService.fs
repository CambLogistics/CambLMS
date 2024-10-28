namespace camblms.server.service

open System.Net.Mail
open System.Text.RegularExpressions
open System.IO
open FSharp.Data.Sql

open camblms.dto
open camblms.server
open camblms.server.config
open camblms.server.database

module ForgotPassService =
    let emailConfig = Config.readEmail ()

    let generateEmail (username: string) (password: string) =
        let mailTemplate =
            try
                File.ReadAllText("templates/email/forgotpass.html")
            with _ ->
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

    let changePasswordByEmail (user: Member) =
        let db = Database.getDataContext ()

        let userEntity =
            query {
                for u in db.Camblogistics.users do
                    where (u.Id = user.Id)
                    exactlyOne
            }

        let newPassword = RandomString.getRandomString 12
        let smtpClient = new SmtpClient(emailConfig.Host)
        smtpClient.Port <- emailConfig.Port

        let message =
            new MailMessage(
                new MailAddress("noreply@" + emailConfig.Domain, emailConfig.SenderName),
                new MailAddress(user.Email)
            )

        message.IsBodyHtml <- true
        message.Body <- generateEmail user.Name newPassword
        message.BodyEncoding <- System.Text.Encoding.UTF8
        message.Subject <- "CambLogistics elfelejtett jelszó"
        message.SubjectEncoding <- System.Text.Encoding.UTF8
        smtpClient.Send message
        userEntity.Password <- newPassword |> UserService.hashPassword
        db.SubmitUpdates()
        message.Dispose()
        smtpClient.Dispose()

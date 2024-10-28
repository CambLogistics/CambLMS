namespace camblms.dto

open WebSharper

[<JavaScript>]
type ForgotPassStatus =
    | MailSent
    | MailError
    | NoSuchUser
    | DatabaseError

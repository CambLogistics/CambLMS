namespace camblms

open WebSharper

type ForgotPassStatus =
        |MailSent
        |NoSuchUser
        |DatabaseError

//TODO: Generate new password and mail it to the user

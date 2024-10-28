namespace camblms.dto

open WebSharper

[<JavaScript>]
type LoginResult =
    | Success of string
    | CredentialError
    | DatabaseError
    | NotApproved

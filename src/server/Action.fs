namespace camblms

open WebSharper

[<JavaScript>]
type ActionResult =
    |Success
    |InvalidSession
    |InsufficientPermissions
    |InactiveUser
    |DatabaseError
    |OtherError of string
namespace camblms.dto

open WebSharper

[<JavaScript>]
type RegisterResult =
    | Success
    | Exists
    | MissingData
    | BadName
    | BadPassword
    | DatabaseError

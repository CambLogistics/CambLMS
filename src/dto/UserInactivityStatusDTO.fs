namespace camblms.dto

open WebSharper

[<JavaScript>]
type UserInactivityStatus =
    { UserName: string
      UserID: int
      Status: bool }

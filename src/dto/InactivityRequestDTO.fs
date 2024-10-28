namespace camblms.dto

open System
open WebSharper

[<JavaScript>]
type InactivityRequest =
    { UserName: string
      UserID: int
      From: DateTime
      To: DateTime
      Reason: string }

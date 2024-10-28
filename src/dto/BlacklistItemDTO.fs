namespace camblms.dto

open WebSharper

[<JavaScript>]
type BlacklistItem =
    { UserName: string
      Role: int
      AccountID: int
      Reason: string
      Comeback: bool }

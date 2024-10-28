namespace camblms.dto

open WebSharper

[<JavaScript>]
type PendingFee =
    { ID: int
      Username: string
      Amount: int }

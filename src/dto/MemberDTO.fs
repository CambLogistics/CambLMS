namespace camblms.dto

open WebSharper

[<JavaScript>]
type Member =
    { Id: int
      Name: string
      Role: Rank
      AccountID: int
      Email: string }

namespace camblms.dto

open WebSharper

[<JavaScript>]
type PendingChange =
    { UserID: int
      OldName: string
      NewName: string }

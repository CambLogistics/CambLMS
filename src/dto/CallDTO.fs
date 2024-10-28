namespace camblms.dto

open WebSharper

[<JavaScript>]
type Call =
    { Type: CallType
      Price: int
      Date: System.DateTime
      ThisWeek: bool
      PreviousWeek: bool
      CurrentTopList: bool }

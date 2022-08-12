namespace camblms

open WebSharper

[<JavaScript>]
type ActionResult =
    |Success
    |Error of System.Exception
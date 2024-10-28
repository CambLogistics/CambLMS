namespace camblms.dto

open WebSharper

[<JavaScript>]
type MemberWithCalls = { User: Member; Calls: Call list }

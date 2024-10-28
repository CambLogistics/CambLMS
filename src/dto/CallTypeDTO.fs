namespace camblms.dto

open WebSharper

[<JavaScript>]
type CallType =
    | Delivery = 0 //Left here for backwards-compatibility reasons
    | Taxi = 1
    | Towing = 2

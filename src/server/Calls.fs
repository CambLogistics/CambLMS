namespace camblms

open WebSharper

[<JavaScript>]
type CallResult =
    |Success
    |InvalidSession
    |DatabaseError

type CallType =
    |Delivery = 0
    |Taxi = 1
    |Towing = 2

type Call = {Type: CallType;Price: int; Date: System.DateTime; ThisWeek: bool; SecondWeek: bool}

module Calls =
    let registerCall sid price (callType:CallType) =
        let user = User.getUserFromSID sid
        match user with
            |None -> InvalidSession
            |Some u ->
                let db = Database.SqlConnection.GetDataContext()
                let call = db.Camblogistics.Calls.Create()
                call.Date <- System.DateTime.Now
                call.Price <- price
                call.UserId <- u.Id
                call.ThisWeek <- (sbyte 1)
                call.SecondWeek <- (sbyte 0)
                call.Type <- int16 callType
                db.SubmitUpdates()
                Success
    let rotateWeek sid =
        if User.verifyAdmin sid |> not then ()
        else
        try
        let db = Database.SqlConnection.GetDataContext()
        query{
            for call in db.Camblogistics.Calls do
            where(call.ThisWeek = (sbyte 1) || call.SecondWeek = (sbyte 1))
            select call
        } |> Seq.iter (
                fun c -> 
                    if c.ThisWeek = (sbyte 1) then 
                        c.ThisWeek <- (sbyte 0)
                        c.SecondWeek <- (sbyte 1)
                    else if c.SecondWeek = (sbyte 1) then c.SecondWeek <- (sbyte 0)
            )
        with
        _ -> ()
    let getCallsOfUser (user:Member) =
        try 
        let db = Database.SqlConnection.GetDataContext()
        query{
            for c in db.Camblogistics.Calls do
            where(user.Id = c.UserId)
            select {Type = enum <| int32 c.Type;Price = c.Price; Date = c.Date; ThisWeek = c.ThisWeek = (sbyte 1);SecondWeek = c.SecondWeek = (sbyte 1)}
        } |> Seq.toList
        with
        _ -> []
    let getCallsBySID sid =
        let user = User.getUserFromSID sid
        match user with
            |None -> []
            |Some u -> getCallsOfUser u
    


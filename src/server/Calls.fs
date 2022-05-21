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

[<JavaScript>]
type CallDuration =
    |All
    |TwoWeeks
    |Weekly

[<JavaScript>]
type Call = {Type: CallType;Price: int; Date: System.DateTime; ThisWeek: bool; PreviousWeek: bool}

[<JavaScript>]
type UserWithCalls = {User:Member;Calls: Call list}

[<JavaScript>]
type Area = {Id:int;Name: string}

module Calls =
    let getAreaList() =
        try
            let db = Database.SqlConnection.GetDataContext()
            query{
                for a in db.Camblogistics.Areas do
                    select((a.Id,a.Name))
            } |> Map.ofSeq
        with
           _ -> Map.ofList []
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
                call.PreviousWeek <- (sbyte 0)
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
            where(call.ThisWeek = (sbyte 1) || call.PreviousWeek = (sbyte 1))
            select call
        } |> Seq.iter (
                fun c -> 
                    if c.ThisWeek = (sbyte 1) then 
                        c.ThisWeek <- (sbyte 0)
                        c.PreviousWeek <- (sbyte 1)
                    else if c.PreviousWeek = (sbyte 1) then c.PreviousWeek <- (sbyte 0)
            )
        db.SubmitUpdates()
        with
        _ -> ()
    let getCallsOfUser (user:Member) =
        try 
        let db = Database.SqlConnection.GetDataContext()
        query{
            for c in db.Camblogistics.Calls do
            where(user.Id = c.UserId)
            select {Type = enum <| int32 c.Type;Price = c.Price; Date = c.Date; ThisWeek = c.ThisWeek = (sbyte 1);PreviousWeek = c.PreviousWeek = (sbyte 1)}
        } |> Seq.toList
        with
        _ -> []
    let getCallsBySID sid =
        let user = User.getUserFromSID sid
        match user with
            |None -> []
            |Some u -> getCallsOfUser u
    [<Rpc>]
    let getUserListWithCalls sid duration =
        async{
                return User.getUserList sid false |> List.map(
                    fun u ->
                        {User = u;Calls = getCallsOfUser u |> 
                                            List.filter
                                                (
                                                    fun c ->
                                                        match duration with
                                                            |All -> true
                                                            |TwoWeeks -> c.PreviousWeek || c.ThisWeek
                                                            |Weekly -> c.ThisWeek
                                                )}
                )
    
        }
    [<Rpc>]
    let doGetUserCalls sid user =
        async{
            if User.verifyAdmin sid |> not then return []
            else return getCallsOfUser user
        }
    [<Rpc>]
    let doRotateWeek sid =
        async{
            return rotateWeek sid
        }
    [<Rpc>]
    let doGetAreaList() =
        async{
            return getAreaList()
        }


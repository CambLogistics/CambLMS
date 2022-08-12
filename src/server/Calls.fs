namespace camblms

open WebSharper

[<JavaScript>]
type CallResult =
    | Success
    | InvalidSession
    | InactiveUser
    | NoPermission
    | DatabaseError

type CallType =
    | Delivery = 0
    | Taxi = 1
    | Towing = 2

[<JavaScript>]
type CallDuration =
    | All
    | TwoWeeks
    | Weekly

[<JavaScript>]
type Call =
    { Type: CallType
      Price: int
      Date: System.DateTime
      ThisWeek: bool
      PreviousWeek: bool }

[<JavaScript>]
type UserWithCalls = { User: Member; Calls: Call list }

[<JavaScript>]
type Area = { Id: int; Name: string }

module Calls =
    let isDoublePrice () =
        try
        let db = Database.SqlConnection.GetDataContext(Database.getConnectionString ())
        let convertedHolidays =
            query {
                for h in db.Camblogistics.holidays do
                    select h
            }
            |> Seq.map (fun h ->
                if h.EveryYear = (sbyte 1) then
                    ((new System.DateTime(System.DateTime.Now.Year, h.StartDate.Month, h.StartDate.Day)),
                    (new System.DateTime(System.DateTime.Now.Year, h.EndDate.Month, h.EndDate.Day)))
                else
                    (h.StartDate,h.EndDate)
                )

        let openToday =
            query {
                for oh in db.Camblogistics.operatinghours do
                    where (LanguagePrimitives.EnumToValue System.DateTime.Now.DayOfWeek = int32 oh.DayOfWeek)
                    select (oh.Opening.Hours, oh.Closing.Hours)
            }

        (query {
            for (o, c) in openToday do
                where (
                    (o > System.DateTime.Now.Hour
                     || c <= System.DateTime.Now.Hour)
                )

                count
         }
         + query {
             for (startDate, endDate) in convertedHolidays do
                 where (
                     startDate <= System.DateTime.Now
                     && endDate > System.DateTime.Now
                 )

                 count
         }) > 0
         with
            _ -> false

    let transformPrice (price:int) callType =
        if isDoublePrice () then
            match callType with
                |CallType.Delivery -> int (float price * 1.25)
                |CallType.Towing -> int (float price * 1.25)
                |CallType.Taxi -> price * 2
                |_ -> price
        else
            price

    let getAreaList () =
        try
            let db = Database.SqlConnection.GetDataContext(Database.getConnectionString ())
            query {
                for a in db.Camblogistics.areas do
                    select ((a.Id, a.Name))
            }
            |> Map.ofSeq
        with
        | _ -> Map.ofList []

    let registerCall sid price (callType: CallType) =
        let permission = 
                match callType with
                        |CallType.Taxi -> Permissions.TaxiCall
                        |CallType.Towing -> Permissions.TowCall
                        |CallType.Delivery -> Permissions.DeliveryCall
                        |_ -> Permissions.Nothing
        let user = User.getUserFromSID sid
        match user with
        | None -> InvalidSession
        | Some u ->
            if not (Permission.checkPermission sid permission) then NoPermission
            else
            if not (Inactivity.getActiveStatus u) then InactiveUser
            else
            let db = Database.SqlConnection.GetDataContext(Database.getConnectionString ())
            let call = db.Camblogistics.calls.Create()
            call.Date <- System.DateTime.Now
            call.Price <- price
            call.UserId <- u.Id
            call.ThisWeek <- (sbyte 1)
            call.PreviousWeek <- (sbyte 0)
            call.Type <- int16 callType
            db.SubmitUpdates()
            Success

    [<Rpc>]
    let rotateWeek sid =
        async{
            if not (Permission.checkPermission sid Permissions.CloseWeek) then
                return ActionResult.InsufficientPermissions
            else
                try
                    let db = Database.SqlConnection.GetDataContext(Database.getConnectionString ())
                    query {
                        for call in db.Camblogistics.calls do
                            where (
                                call.ThisWeek = (sbyte 1)
                                || call.PreviousWeek = (sbyte 1)
                            )
                            select call
                    }
                    |> Seq.iter (fun c ->
                        if c.ThisWeek = (sbyte 1) then
                            c.ThisWeek <- (sbyte 0)
                            c.PreviousWeek <- (sbyte 1)
                        else if c.PreviousWeek = (sbyte 1) then
                            c.PreviousWeek <- (sbyte 0)
                        )
                    db.SubmitUpdates()
                    return ActionResult.Success
                with
                    | _ -> return ActionResult.DatabaseError
        }

    let getCallsOfUser (user: Member) =
        try
            let db = Database.SqlConnection.GetDataContext(Database.getConnectionString ())
            query {
                for c in db.Camblogistics.calls do
                    where (user.Id = c.UserId)
                    select
                        { Type = enum <| int32 c.Type
                          Price = c.Price
                          Date = c.Date
                          ThisWeek = c.ThisWeek = (sbyte 1)
                          PreviousWeek = c.PreviousWeek = (sbyte 1) }
            }
            |> Seq.toList
        with
        | _ -> []

    let getCallsBySID sid =
            let user = User.getUserFromSID sid
            match user with
            | None -> []
            | Some u -> getCallsOfUser u

    [<Rpc>]
    let getUserListWithCalls sid duration =
        async {
            return
                UserOperations.getUserList sid false false
                |> List.map (fun u ->
                    { User = u
                      Calls =
                        getCallsOfUser u
                        |> List.filter (fun c ->
                            match duration with
                            | All -> true
                            | TwoWeeks -> c.PreviousWeek || c.ThisWeek
                            | Weekly -> c.ThisWeek) }) |> List.sortByDescending (fun u -> List.length u.Calls)

        }

    [<Rpc>]
    let doGetDPStatus () = async { return isDoublePrice () }

    [<Rpc>]
    let doGetUserCalls sid user =
        async {
            if not (Permission.checkPermission sid Permissions.ViewCallCount) then
                return []
            else
                return getCallsOfUser user
        }

    [<Rpc>]
    let doGetAreaList () = async { return getAreaList () }

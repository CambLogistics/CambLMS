namespace camblms

open WebSharper

type CallType =
    | Delivery = 0 //Left here for backwards-compatibility reasons
    | Taxi = 1
    | Towing = 2

[<JavaScript>]
type CallDuration =
    | All
    | TopList
    | TwoWeeks
    | Weekly

[<JavaScript>]
type Call =
    { Type: CallType
      Price: int
      Date: System.DateTime
      ThisWeek: bool
      PreviousWeek: bool
      CurrentTopList: bool }

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
            e -> failwith e.Message

    let transformPrice (price:int) callType =
      try
        if isDoublePrice () then
            match callType with
                |CallType.Towing -> int (float price * 1.25)
                |CallType.Taxi -> price * 2
                |_ -> price
        else
            price
      with
        e -> failwith e.Message
    [<Rpc>]
    let getAreaList () =
        async{
            try
                let db = Database.SqlConnection.GetDataContext(Database.getConnectionString ())
                return Ok( query {
                    for a in db.Camblogistics.areas do
                        select ((a.Id, a.Name))
                }
                |> Map.ofSeq)
            with
            | e -> return Error e.Message
        }

    let registerCall sid price (callType: CallType) =
        let permission = 
                match callType with
                        |CallType.Taxi -> Permissions.TaxiCall
                        |CallType.Towing -> Permissions.TowCall
                        |CallType.Delivery -> Permissions.Admin //Left here (disabled) for backwards-compatibility reasons
                        |_ -> Permissions.Admin
        let user = User.getUserFromSID sid
        match user with
        | None -> ActionResult.InvalidSession
        | Some u ->
            if not (Permission.checkPermission sid permission) then ActionResult.InsufficientPermissions
            else
            if not (Inactivity.getActiveStatus u) then ActionResult.InactiveUser
            else
            let db = Database.SqlConnection.GetDataContext(Database.getConnectionString ())
            let call = db.Camblogistics.calls.Create()
            call.Date <- System.DateTime.Now
            call.Price <- price
            call.UserId <- u.Id
            call.ThisWeek <- (sbyte 1)
            call.PreviousWeek <- (sbyte 0)
            call.CurrentTopList <- (sbyte 1)
            call.Type <- int16 callType
            db.SubmitUpdates()
            ActionResult.Success
   

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
    [<Rpc>]
    let rotateTopList sid =
        async{
            if not (Permission.checkPermission sid Permissions.CloseWeek) then
                return ActionResult.InsufficientPermissions
            else
                try
                    let db = Database.SqlConnection.GetDataContext(Database.getConnectionString ())
                    query {
                        for call in db.Camblogistics.calls do
                            where (
                                call.CurrentTopList = (sbyte 1)
                            )
                            select call
                    }
                    |> Seq.iter (fun c ->
                        c.CurrentTopList <- sbyte 0
                        )
                    db.SubmitUpdates()
                    return ActionResult.Success
                with
                    | _ -> return ActionResult.DatabaseError
        }

    let getCallsOfUser (user: Member) =
        try
            let db = Database.SqlConnection.GetDataContext(Database.getConnectionString ())
            Ok (query {
                for c in db.Camblogistics.calls do
                    where (user.Id = c.UserId)
                    select
                        { Type = enum <| int32 c.Type
                          Price = c.Price
                          Date = c.Date
                          ThisWeek = c.ThisWeek = (sbyte 1)
                          PreviousWeek = c.PreviousWeek = (sbyte 1) 
                          CurrentTopList = c.CurrentTopList = (sbyte 1)}
            }
            |> Seq.toList
          )
        with
        | e -> Error e.Message

    let getCallsBySID sid =
            let user = User.getUserFromSID sid
            match user with
            | None -> Error "Nem létező felhasználó hívásait kérted le!"
            | Some u -> getCallsOfUser u

    [<Rpc>]
    let getUserListWithCalls sid duration =
        async {
            try
            let! users = UserOperations.getUserList sid false false
            match users with
                |Ok ul ->
                    return
                        Ok (ul
                            |> List.map (fun u ->
                            { User = u
                              Calls =
                                match getCallsOfUser u with
                                |Ok c -> c
                                        |> List.filter (fun c ->
                                        match duration with
                                        |All -> true
                                        |TopList -> c.CurrentTopList
                                        |TwoWeeks -> c.PreviousWeek || c.ThisWeek
                                        |Weekly -> c.ThisWeek) 
                                |Error e -> failwith e
                            }) |> List.sortByDescending (fun u -> List.length u.Calls)
                        )
                |Error e -> return Error e
            with
            e -> return Error e.Message

        }
    //The following function is intended for the info page -- no RPC
    let getWeeklyCallPercentage sid =
            let user = User.getUserFromSID sid
            match user with
                |None -> 0
                |Some u ->
                    let db = Database.getDataContext()
                    let callCount =
                        query{
                            for c in db.Camblogistics.calls do
                                where(c.UserId = u.Id && (c.ThisWeek = (sbyte 1)))
                                count
                        } |> float32
                    let callMin = 
                        (query{
                            for r in db.Camblogistics.requiredcalls do
                                where(r.RoleId = u.Role)
                                exactlyOne
                        }).Calls |> float32
                    if callMin = 0.0f then 100
                    else ceil ((callCount/callMin)*100.0f) |> int
    [<Rpc>]
    let clientGetDPStatus () = async { return isDoublePrice () }

namespace camblms.server.service

open WebSharper
open FSharp.Data.Sql

open camblms.dto
open camblms.server.database

module CallsService =
    let isDoublePrice () =
        try
            let db = Database.getDataContext ()

            let convertedHolidays =
                query {
                    for h in db.Camblogistics.holidays do
                        select h
                }
                |> Seq.map (fun h ->
                    if h.EveryYear = (sbyte 1) then
                        let newStartDate =
                            new System.DateTime(System.DateTime.Now.Year, h.StartDate.Month, h.StartDate.Day)

                        ((newStartDate), ((newStartDate.Add(h.EndDate.Subtract(h.StartDate)))))
                    else
                        (h.StartDate, h.EndDate))

            let openToday =
                query {
                    for oh in db.Camblogistics.operatinghours do
                        where (LanguagePrimitives.EnumToValue System.DateTime.Now.DayOfWeek = int32 oh.DayOfWeek)
                        select (oh.Opening.Hours, oh.Closing.Hours)
                }

            (query {
                for (o, c) in openToday do
                    where ((o > System.DateTime.Now.Hour || c <= System.DateTime.Now.Hour))

                    count
             }
             + query {
                 for (startDate, endDate) in convertedHolidays do
                     where (
                         startDate <= System.DateTime.Now
                         && endDate.AddHours(23).AddMinutes(59).AddSeconds(59) > System.DateTime.Now
                     )

                     count
             }) > 0
        with e ->
            failwith e.Message

    let transformPrice (price: int) callType =
        try
            if isDoublePrice () then
                match callType with
                | CallType.Towing -> int (round ((float (price) * 1.251) / 100.0) * 100.0)
                | CallType.Taxi -> int (round ((float (price) * 1.251) / 100.0) * 100.0)
                | _ -> price
            else
                price
        with e ->
            failwith e.Message

    let getAreaList () =
        let db = Database.getDataContext ()

        query {
            for a in db.Camblogistics.areas do
                select ((a.Id, a.Name))
        }
        |> Map.ofSeq

    let registerCall (user: Member) price (callType: CallType) =
        let db = Database.getDataContext ()
        let call = db.Camblogistics.calls.Create()
        call.Date <- System.DateTime.Now
        call.Price <- price
        call.UserId <- user.Id
        call.ThisWeek <- (sbyte 1)
        call.PreviousWeek <- (sbyte 0)
        call.CurrentTopList <- (sbyte 1)
        call.Type <- int16 callType
        db.SubmitUpdates()

    let rotateWeek () =
        let db = Database.getDataContext ()

        query {
            for call in db.Camblogistics.calls do
                where (call.ThisWeek = (sbyte 1) || call.PreviousWeek = (sbyte 1))
                select call
        }
        |> Seq.iter (fun c ->
            if c.ThisWeek = (sbyte 1) then
                c.ThisWeek <- (sbyte 0)
                c.PreviousWeek <- (sbyte 1)
            else if c.PreviousWeek = (sbyte 1) then
                c.PreviousWeek <- (sbyte 0))

        db.SubmitUpdates()

    let rotateTopList () =
        let db = Database.getDataContext ()

        query {
            for call in db.Camblogistics.calls do
                where (call.CurrentTopList = (sbyte 1))
                select call
        }
        |> Seq.iter (fun c -> c.CurrentTopList <- sbyte 0)

        db.SubmitUpdates()

    let getCallsOfUser (user: Member) =
        let db = Database.getDataContext ()

        query {
            for c in db.Camblogistics.calls do
                where (user.Id = c.UserId)

                select
                    { Type = enum <| int32 c.Type
                      Price = c.Price
                      Date = c.Date
                      ThisWeek = c.ThisWeek = (sbyte 1)
                      PreviousWeek = c.PreviousWeek = (sbyte 1)
                      CurrentTopList = c.CurrentTopList = (sbyte 1) }
        }
        |> Seq.toList



    let getUserListWithCalls duration =
        UserService.getUserList false false
        |> List.map (fun u ->
            let userCalls = getCallsOfUser u

            { User = u
              Calls =
                userCalls
                |> List.filter (fun c ->
                    match duration with
                    | All -> true
                    | TopList -> c.CurrentTopList
                    | TwoWeeks -> c.PreviousWeek || c.ThisWeek
                    | Weekly -> c.ThisWeek) })

    let getWeeklyCallPercentage (user: Member) =
        let db = Database.getDataContext ()

        let callCount =
            query {
                for c in db.Camblogistics.calls do
                    where (c.UserId = user.Id && (c.ThisWeek = (sbyte 1)))
                    count
            }
            |> float32

        let callMin =
            (query {
                for r in db.Camblogistics.requiredcalls do
                    where (r.RoleId = user.Role.Level)
                    exactlyOne
            })
                .Calls
            |> float32

        if callMin = 0.0f then
            100
        else
            ceil ((callCount / callMin) * 100.0f) |> int

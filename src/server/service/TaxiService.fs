namespace camblms.server.service

open FSharp.Data.Sql

open camblms.dto
open camblms.server.database

module TaxiService =
    let calculatePrice source dest =
        try
            let db = Database.getDataContext ()

            let route =
                query {
                    for route in db.Camblogistics.taxiprices do
                        where (
                            (route.Source = source && route.Destination = dest)
                            || (route.Source = dest && route.Destination = source)
                        )

                        exactlyOne
                }

            CallsService.transformPrice route.Price CallType.Taxi
        with _ ->
            0

    let getInfo user =
        let allCalls = CallsService.getCallsOfUser user
        let calls = allCalls |> List.filter (fun c -> c.Type = CallType.Taxi)

        query {
            for c in calls do
                where (c.Date.Day = System.DateTime.Today.Day)
                count
        },
        query {
            for c in calls do
                where (c.ThisWeek)
                count
        },
        query {
            for c in calls do
                where (c.ThisWeek || c.PreviousWeek)
                count
        },
        List.length calls

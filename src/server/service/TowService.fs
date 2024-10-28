namespace camblms.server.service

open FSharp.Data.Sql

open camblms.dto
open camblms.server.database

module TowService =
    let calculatePrice source dest =
        try
            let db = Database.getDataContext ()

            let route =
                query {
                    for route in db.Camblogistics.towprices do
                        where (
                            (route.Source = source && route.Destination = dest)
                            || (route.Source = dest && route.Destination = source)
                        )

                        exactlyOne
                }

            CallsService.transformPrice route.Price CallType.Towing
        with _ ->
            0

    let getInfo user =
        async {
            let allCalls = CallsService.getCallsOfUser user
            let calls = allCalls |> List.filter (fun c -> c.Type = CallType.Towing)

            return
                (query {
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
                 List.length calls)
        }

    let getGarageList () =
        async {
            try
                let db = Database.getDataContext ()

                let! garages =
                    query {
                        for g in db.Camblogistics.towgarages do
                            select ((g.Id, g.Name))
                    }
                    |> Seq.executeQueryAsync
                    |> Async.AwaitTask

                return garages |> Map.ofSeq
            with _ ->
                return Map.ofList []
        }

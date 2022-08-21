namespace camblms

open WebSharper

module Taxi =
    let calculatePrice source dest =
        try
        let db = Database.getDataContext()
        Calls.transformPrice (query{
            for route in db.Camblogistics.taxiprices do
            where((route.Source = source && route.Destination = dest) ||
                    (route.Source = dest && route.Destination = source))
            exactlyOne
            }).Price CallType.Taxi
        with
            _ -> 0
    [<Rpc>]
    let doCalculatePrice source dest =
        async{
            return calculatePrice source dest
        }
    [<Rpc>]
    let submitCall (sid, source, dest) =
        async{
            try
                return calculatePrice source dest |> Calls.registerCall sid <| CallType.Taxi 
            with
                _ -> return ActionResult.DatabaseError
        }
    let getInfo sid =
        let calls = 
            match Calls.getCallsBySID sid with
                |Ok c -> c |> List.filter (fun c -> c.Type = CallType.Taxi)
                |Error e -> failwith e
        ( 
            query{
                for c in calls do
                where (c.Date.Day = System.DateTime.Today.Day)
                count
            },
            query{
                for c in calls do
                where (c.ThisWeek)
                count
            },
            query{
                for c in calls do
                where(c.ThisWeek || c.PreviousWeek)
                count
            },
            List.length calls
        )
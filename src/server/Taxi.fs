namespace camblms

open WebSharper

module Taxi =
    let calculatePrice source dest =
        let db = Database.SqlConnection.GetDataContext()
        (query{
            for route in db.Camblogistics.TaxiPrices do
            where((route.Source = source && route.Destination = dest) ||
                    (route.Source = dest && route.Destination = source))
            exactlyOne
            }).Price
    [<Rpc>]
    let doCalculatePrice source dest =
        async{
            return calculatePrice source dest
        }
    [<Rpc>]
    let submitCall sid source dest=
        async{
        try
            return calculatePrice source dest |> Calls.registerCall sid <| CallType.Taxi 
        with
            _ -> return CallResult.DatabaseError
        }
    let getInfo sid =
        let calls = Calls.getCallsBySID sid |> List.filter (fun c -> c.Type = CallType.Taxi)
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
                where(c.ThisWeek || c.SecondWeek)
                count
            }
        )
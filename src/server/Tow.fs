namespace camblms

open WebSharper

module Tow =
    let getGarageList() =
        try
            let db = Database.SqlConnection.GetDataContext()
            query{
                for g in db.Camblogistics.TowGarages do
                select((g.Id,g.Name))
            } |> Map.ofSeq
        with
            _ -> Map.ofList []
    let calculatePrice source dest =
        try
        let db = Database.SqlConnection.GetDataContext()
        (query{
            for route in db.Camblogistics.TowPrices do
            where(route.Source = source && route.Destination = dest)
            exactlyOne
            }).Price
        with
            _ -> 0
    [<Rpc>]
    let doCalculatePrice source dest =
        async{
            return calculatePrice source dest
        }
    [<Rpc>]
    let submitCall sid source dest=
        async{
        try
            return calculatePrice source dest |> Calls.registerCall sid <| CallType.Towing
        with
            _ -> return CallResult.DatabaseError
        }
    [<Rpc>]
    let doGetGarageList() =
        async{
            return getGarageList()
        }
    let getInfo sid =
        let calls = Calls.getCallsBySID sid |> List.filter (fun c -> c.Type = CallType.Towing)
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
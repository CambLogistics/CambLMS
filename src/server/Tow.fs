namespace camblms

open WebSharper

module Tow =
    let getGarageList() =
        try
            let db = Database.getDataContext()
            query{
                for g in db.Camblogistics.towgarages do
                select((g.Id,g.Name))
            } |> Map.ofSeq
        with
            _ -> Map.ofList []
    let calculatePrice source dest =
        try
        let db = Database.getDataContext()
        Calls.transformPrice (query{
            for route in db.Camblogistics.towprices do
            where(route.Source = source && route.Destination = dest)
            exactlyOne
            }).Price CallType.Towing
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
            return calculatePrice source dest |> Calls.registerCall sid <| CallType.Towing
        with
            _ -> return ActionResult.DatabaseError
        }
    [<Rpc>]
    let doGetGarageList() =
        async{
            return getGarageList()
        }
    let getInfo sid =
        let calls = 
            match Calls.getCallsBySID sid with
                |Ok c -> 
                    c |> List.filter (fun c -> c.Type = CallType.Towing)
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
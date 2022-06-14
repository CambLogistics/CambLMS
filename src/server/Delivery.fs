namespace camblms

open WebSharper

[<JavaScript>]
type DeliveryType = {ID: int;Name: string; Price: int}

module Delivery =
    let getTypeList() =
        try
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        query{
            for t in db.Camblogistics.deliverytypes do
                join p in db.Camblogistics.deliveryprices on (t.Id = p.Type)
                select({ID=t.Id;Name=t.Name;Price=p.Price})
        } |> Seq.toList
        with
            _ -> []
    let calculatePrice deliveryType =
        try
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        Calls.transformPrice (query{
            for price in db.Camblogistics.deliveryprices do
            where(deliveryType = price.Type)
            exactlyOne
            }).Price CallType.Delivery
        with
            _ -> 0
    [<Rpc>]
    let doCalculatePrice dt =
        async{
            return calculatePrice dt
        }
    [<Rpc>]
    let submitCall sid dt =
        async{
            try
                return calculatePrice dt |> Calls.registerCall sid <| CallType.Delivery
            with
                _ -> return CallResult.DatabaseError
        }
    [<Rpc>]
    let doGetTypeList() =
        async{
            return getTypeList()
        }
    let getInfo sid =
        let calls = Calls.getCallsBySID sid |> List.filter (fun c -> c.Type = CallType.Delivery)
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
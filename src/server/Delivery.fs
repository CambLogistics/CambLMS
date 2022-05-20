namespace camblms

open WebSharper

module Delivery =
    let calculatePrice deliveryType =
        let db = Database.SqlConnection.GetDataContext()
        (query{
            for price in db.Camblogistics.DeliveryPrices do
            where(deliveryType = price.Type)
            exactlyOne
            }).Price
    [<Rpc>]
    let doCalculatePrice dt =
        async{
            return calculatePrice dt
        }
    [<Rpc>]
    let submitCall sid dt=
        async{
        try
            return calculatePrice dt |> Calls.registerCall sid <| CallType.Delivery
        with
            _ -> return CallResult.DatabaseError
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
            }
        )
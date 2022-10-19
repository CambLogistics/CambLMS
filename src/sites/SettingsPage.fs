namespace camblms

open WebSharper
open WebSharper.Sitelets

module SettingsPage =
    let RenderPage (ctx:Context<EndPoint>) user =
        let sessionID = (ctx.Request.Cookies.Item "clms_sid").Value
        let callsOfUser = Calls.getCallsBySID sessionID
        SiteParts.SettingsTemplate()
            .AccID(string user.AccountID)
            .Name(user.Name)
            .Cars(
                 List.fold (
                                fun s rn -> (s + " " + rn)
                                ) "" (
                                  match Cars.getCarsOfKeyHolder sessionID with
                                    |Ok c -> c
                                    |Error e -> failwith e
                                  )
            )
             .Rank(
                        let db = Database.getDataContext()
                        (query{
                            for r in db.Camblogistics.roles do
                            where(r.Id = user.Role)
                            exactlyOne
                        }).Name
                    )
            .MoneySum(
                match callsOfUser with
                    |Ok calls ->
                        (calls |> List.sumBy (fun c -> c.Price) |> string) + " $"
                    |Error e -> "Hiba"
            )
            .TwoWeekMoney(
                match callsOfUser with
                    |Ok calls ->
                        (calls |> List.filter (fun c -> c.ThisWeek || c.PreviousWeek) |> List.sumBy (fun c -> c.Price) |> string) + " $"
                    |Error e -> "Hiba"
            )
            .TaxiSum(
                match callsOfUser with
                    |Ok calls ->
                        calls |> List.filter (fun c -> c.Type = CallType.Taxi) |> List.length |> string
                    |Error e -> "Hiba"
            )
            .TowSum(
                match callsOfUser with
                    |Ok calls ->
                        calls |> List.filter (fun c -> c.Type = CallType.Towing) |> List.length |> string
                    |Error e -> "Hiba"
            )
            .Contract("#") //Not implemented
            .Doc()
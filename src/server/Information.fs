namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.Sitelets

module Information =
    let RenderPage (ctx:Context<EndPoint>) =
      try
        let sessionID = ctx.Request.Cookies.Item "clms_sid"
        let user = 
            match sessionID with
                |Some s -> User.getUserFromSID s
                |None -> None 
        match user with
            |None -> Doc.Empty
            |Some u ->
                let (taxiDaily,taxiWeekly,taxiBiWeekly,taxiAll) = Taxi.getInfo sessionID.Value
                let (towingDaily,towingWeekly,towingBiWeekly,towingAll) = Tow.getInfo sessionID.Value
                let callsOfUser = 
                  match Calls.getCallsBySID sessionID.Value with
                    |Ok c -> c
                    |Error e ->  failwith e
                SiteParts.InfoTemplate()
                    .Name(u.Name)
                    .Rank(
                        let db = Database.getDataContext()
                        (query{
                            for r in db.Camblogistics.roles do
                            where(r.Id = u.Role)
                            exactlyOne
                        }).Name
                    )
                    .AccID(string u.AccountID)
                    .TaxiDaily(string taxiDaily)
                    .TaxiWeekly(string taxiWeekly)
                    .TaxiSum(string taxiAll)
                    .TowDaily(string towingDaily)
                    .TowWeekly(string towingWeekly)
                    .TowSum(string towingAll)
                    .Cars(
                        List.fold (
                                fun s rn -> (s + " " + rn)
                                ) "" (
                                  match Cars.getCarsOfKeyHolder sessionID.Value with
                                    |Ok c -> c
                                    |Error e -> failwith e
                                  )
                    )
                    .MoneySum((callsOfUser |> List.sumBy (fun c -> c.Price) |> string) + " $")
                    .TwoWeekMoney((callsOfUser |> List.filter (fun c -> c.PreviousWeek || c.ThisWeek) |> List.sumBy (fun c -> c.Price) |> string) + " $")
                    .Doc()
      with
        e -> SiteParts.NotFoundTemplate().ErrorMessage("Hiba az információs oldal betöltése közben! Értesítsd a (műszaki) igazgatót!").Doc()

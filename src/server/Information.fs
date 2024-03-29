namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.Sitelets

module Information =
    let RenderPage (ctx:Context<EndPoint>) =
      try
        let sessionID = ctx.Request.Cookies.Item "clms_sid"
        let callPercentage = Calls.getWeeklyCallPercentage sessionID.Value
        let user = 
            match sessionID with
                |Some s -> User.getUserFromSID s
                |None -> None 
        match user with
            |None -> Doc.Empty
            |Some u ->
                
                let callsOfUser = 
                  match Calls.getCallsBySID sessionID.Value with
                    |Ok c -> c
                    |Error e ->  failwith e
                SiteParts.InfoTemplate()
                    .Rank(
                        let db = Database.getDataContext()
                        (query{
                            for r in db.Camblogistics.roles do
                            where(r.Id = u.Role)
                            exactlyOne
                        }).Name
                    )
                    .Cars(
                        List.fold (
                                fun s rn -> (s + " " + rn)
                                ) "" (
                                  match Cars.getCarsOfKeyHolder sessionID.Value with
                                    |Ok c -> c
                                    |Error e -> failwith e
                                  )
                    )
                    .MoneySum((callsOfUser |> List.sumBy (fun c -> c.Price) |> string))
                    .CallSum(List.length callsOfUser |> string)
                    .WeeklyCallWidth(string <| (if callPercentage <= 100 then callPercentage else 100))
                    .WeeklyCallPercentage(string <| callPercentage)
                    .RecentCalls(
                      callsOfUser |> List.sortByDescending (fun c -> c.Date) |>  List.take (if List.length callsOfUser >= 5 then 5 else List.length callsOfUser) |> List.map (
                        fun c -> 
                          SiteParts.InfoTemplate.CallItem()
                            .Date(sprintf "%04d-%02d-%02d %02d:%02d" c.Date.Year c.Date.Month c.Date.Day c.Date.Hour c.Date.Minute)
                            .Type(
                              match c.Type with
                                |CallType.Taxi -> "Taxi"
                                |CallType.Towing -> "Vonti"
                                |CallType.Delivery -> "Fuvar (ELAVULT!)"
                                |_ -> "Ismeretlen"
                            )
                            .Price(sprintf "%d" c.Price)
                            .Doc()
                      ) |> Doc.Concat
                    )
                    .Doc()
      with
        e -> SiteParts.NotFoundTemplate().ErrorMessage("Hiba az információs oldal betöltése közben! Értesítsd a (műszaki) igazgatót!").Doc()

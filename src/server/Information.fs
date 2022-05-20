namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.Sitelets

module Information =
    let RenderPage (ctx:Context<EndPoint>) =
        let sessionID = ctx.Request.Cookies.Item "clms_sid"
        let user = 
            match sessionID with
                |Some s -> User.getUserFromSID s
                |None -> None 
        match user with
            |None -> Doc.Empty
            |Some u ->
                let (taxiDaily,taxiWeekly,taxiBiWeekly,taxiAll) = Taxi.getInfo sessionID.Value
                let (deliveryDaily,deliveryWeekly,deliveryBiWeekly,deliveryAll) = Delivery.getInfo sessionID.Value
                let (towingDaily,towingWeekly,towingBiWeekly,towingAll) = Tow.getInfo sessionID.Value
                SiteParts.InfoTemplate()
                    .Name(u.Name)
                    .Rank(
                        (query{
                            for r in User.getRankList do
                            where(r.Level = u.Role)
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
                    .TransDaily(string deliveryDaily)
                    .TransWeekly(string deliveryWeekly)
                    .TransSum(string deliveryAll)
                    .Cars(
                        List.fold (
                                fun s rn -> Doc.Concat (SiteParts.InfoTemplate.CarItem().RegNum(rn).Doc()::[s])
                                ) Doc.Empty (Cars.getCarsOfKeyHolder sessionID.Value)
                    )
                    .Doc()
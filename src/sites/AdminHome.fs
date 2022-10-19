namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module AdminHome =
    let weeklyUserList = ListModel.Create (fun (u:UserWithCalls) -> u) []
    let topList = ListModel.Create (fun (u:UserWithCalls*int) -> u) []
    let allTaxis = Var.Create 0
    let occupiedTaxis = Var.Create 0
    let allTowTrucks = Var.Create 0
    let occupiedTowTrucks = Var.Create 0
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
    let rankList = Var.Create [{Level = 0;Name="Beszállító"}]
    let mutable canClose = false
    let updateWeeklyUserList() =
        async{
            let! list = Calls.getUserListWithCalls sessionID CallDuration.Weekly
            let! cc = Permission.doCheckPermission sessionID Permissions.CloseWeek
            canClose <- cc
            if not cc then JavaScript.JS.Document.GetElementById("closeweek").SetAttribute("style","display: none;")
            match list with
                |Ok l -> l |> weeklyUserList.Set
                |Error e -> Feedback.giveFeedback true e
        } |> Async.Start
    let updateCarData() =
        async{
            let! taxiStat = Cars.getCarCountByWorkType sessionID CarWorkType.Taxi
            let! towingStat = Cars.getCarCountByWorkType sessionID CarWorkType.Tow
            match taxiStat with
                |Ok (all,occupied) ->
                    allTaxis.Set all
                    occupiedTaxis.Set occupied
                    match towingStat with
                        |Ok(tAll,tOcc) ->
                            allTowTrucks.Set tAll
                            occupiedTowTrucks.Set tOcc
                        |Error e -> Feedback.giveFeedback true e
                |Error e -> Feedback.giveFeedback true e
        } |> Async.Start
    let updateTopList() =
        async{
            let! list = Calls.getUserListWithCalls sessionID CallDuration.TopList
            let! cc = Permission.doCheckPermission sessionID Permissions.CloseWeek
            canClose <- cc
            if not cc then JavaScript.JS.Document.GetElementById("closemonth").SetAttribute("style","display: none;")
            match list with
                |Ok l -> l |> List.map (fun u -> (u,u.Calls |> List.map (fun c -> c.Price) |> List.sum)) |> List.sortByDescending (fun (_,money) -> money) |> topList.Set
                |Error e -> Feedback.giveFeedback true e
        } |> Async.Start
    let updateRankList() =
        async{
            let! list = User.getRankList()
            match list with
                |Ok l -> l |> rankList.Set
                |Error e -> Feedback.giveFeedback true e
        } |> Async.Start
    let RenderPage() =
        updateRankList()
        updateWeeklyUserList()
        updateTopList()
        updateCarData()
        SiteParts.AdminHomeTemplate()
            .CallMemberList(
                weeklyUserList.View |> Doc.BindSeqCached (
                    fun u ->
                        SiteParts.AdminHomeTemplate.CallMember()
                            .Name(u.User.Name)
                            .CallNum(u.Calls |> List.length |> string)
                            .Rank(
                                (query{
                                    for rank in rankList.Value do
                                    where(rank.Level = u.User.Role)
                                    exactlyOne
                                }).Name
                            )
                            .Doc()
                )
            )
            .TopMemberList(
                topList.View |> Doc.BindSeqCached (
                    fun (u,m) ->
                        SiteParts.AdminHomeTemplate.TopMember()
                            .Name(u.User.Name)
                            .CallMonth(string <| List.length u.Calls)
                            .MonthMoney(string m)
                            .Doc()
                )
            )
            .AllTaxis(string allTaxis.Value)
            .AllTowingTrucks(string allTowTrucks.Value)
            .TaxiCar(string occupiedTaxis.Value)
            .TowTruck(string occupiedTowTrucks.Value)
            .CashSum(weeklyUserList.Value |> Seq.map (fun uc -> uc.Calls) |> Seq.map (fun cl -> cl |> List.map (fun c -> c.Price)) |> Seq.map (fun p -> List.sum p) |> Seq.sum |> string)
            .CloseWeek(
                fun _ ->
                    if not canClose then ()
                    else
                        ActionDispatcher.RunAction Calls.rotateWeek sessionID (Some updateWeeklyUserList)
            )
            .CloseMonth(
                 fun _ ->
                    if not canClose then ()
                    else
                        ActionDispatcher.RunAction Calls.rotateTopList sessionID (Some updateWeeklyUserList)
            )
            .Doc()

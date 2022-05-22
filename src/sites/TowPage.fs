namespace camblms

open WebSharper
open WebSharper.UI

[<JavaScript>]
type TowRoute = {Source: int; Dest: int}

[<JavaScript>]
module TowPage =
    let areaList = Var.Create <| Map.ofList [(0,"")]
    let garageList = Var.Create <| Map.ofList [(0,"")]
    let selectedRoute = Var.Create {Source = -1;Dest = -1}
    let updateAreaList() =
        async{
            let! list = Calls.doGetAreaList()
            areaList.Set list
        } |> Async.Start
    let updateGarageList() =
        async{
            let! list = Tow.doGetGarageList()
            garageList.Set list
        } |> Async.Start
    let getAreaName id =
        match Map.tryFind id areaList.Value with
        |None -> ""
        |Some s -> s
    let getGarageName id =
        match Map.tryFind id garageList.Value with
        |None -> ""
        |Some s -> s
    let RenderPage() =
        updateAreaList()
        updateGarageList()
        SiteParts.TowTemplate()
            .Price(
                 selectedRoute.View.MapAsync(
                        fun r ->
                            async{
                                if r.Source = -1 && r.Dest = -1 then 
                                    JavaScript.JS.Document.GetElementById("Submit").SetAttribute("disabled","true")
                                    return "Jelenleg nincs kiválasztva útvonal."
                                else 
                                    let! price = Tow.doCalculatePrice r.Source r.Dest
                                    if price > 0 then JavaScript.JS.Document.GetElementById("Submit").RemoveAttribute("disabled")
                                    return (getAreaName r.Source + " - " + getGarageName r.Dest + ": " + string price)
                            } 
                )
            )
            .SelectAP(fun e -> selectedRoute.Set {selectedRoute.Value with Source = 3})
            .SelectOS(fun e -> selectedRoute.Set {selectedRoute.Value with Source = 1})
            .SelectSF(fun e -> selectedRoute.Set {selectedRoute.Value with Source = 2})
            .SelectLS(fun e -> selectedRoute.Set {selectedRoute.Value with Source = 0})
            .SelectCh(fun e -> selectedRoute.Set {selectedRoute.Value with Source = 4})
            .SelectBS(fun e -> selectedRoute.Set {selectedRoute.Value with Source = 5})
            .SelectFix(fun e -> selectedRoute.Set {selectedRoute.Value with Dest = 1})
            .SelectBMS(fun e -> selectedRoute.Set {selectedRoute.Value with Dest = 0})
            .SelectJunk(fun e -> selectedRoute.Set {selectedRoute.Value with Dest = 2})
            .Submit(
                 fun e ->
                    async{
                        let sessionID = JavaScript.Cookies.Get("clms_sid").Value
                        let! callResult = Tow.submitCall sessionID selectedRoute.Value.Source selectedRoute.Value.Dest
                        match callResult with
                            |CallResult.Success -> 
                                selectedRoute.Set {Source = -1; Dest = -1}
                                Feedback.giveFeedback false "Sikeres művelet!"
                                JavaScript.JS.Window.Location.Replace "/"
                            |CallResult.InvalidSession -> Feedback.giveFeedback true "Érvénytelen munkamenet. Jelentkezz be és ki újra!"
                            |CallResult.DatabaseError -> Feedback.giveFeedback true "Adatbázishiba. Értesítsd a (műszaki) igazgatót!"
                    } |> Async.Start
            )
            .Reset(
                fun e -> 
                    selectedRoute.Set {Source = -1;Dest = -1}
                    JavaScript.JS.Window.Location.Replace "/"
                )
            .Doc()
namespace camblms

open WebSharper
open WebSharper.UI

[<JavaScript>]
type Route = {Source:int; Dest:int}

[<JavaScript>]
module TaxiPage =
    let selectedRoute = Var.Create {Source = -1; Dest = -1}
    let areaList = Var.Create [{Id= -1;Name="Senkiföldje"}]
    let updateAreaList() =
        async{
            let! list = Calls.doGetAreaList()
            areaList.Set list
        } |> Async.Start
    let getAreaName id =
        try
        query{
            for a in areaList.Value do
            where(a.Id = id)
            select a.Name
            exactlyOne
        }
        with
            _ -> ""
    let RenderPage() =
        updateAreaList()
        SiteParts.TaxiTemplate()
            .PriceDisplay(
                selectedRoute.View.MapAsync(
                        fun r ->
                            async{
                                if r.Source = -1 && r.Dest = -1 then 
                                    JavaScript.JS.Document.GetElementById("Submit").SetAttribute("disabled","true")
                                    return "Jelenleg nincs kiválasztva útvonal."
                                else 
                                    let! price = Taxi.doCalculatePrice r.Source r.Dest
                                    if price > 0 then JavaScript.JS.Document.GetElementById("Submit").SetAttribute("disabled","false")
                                    return (getAreaName r.Source + " - " + getAreaName r.Dest + ": " + string price)
                            } 
                )
            )
            .SelectAP(
                fun e ->
                    if e.Target.ParentElement.GetAttribute("name") = "image-map-source" then
                        selectedRoute.Set {selectedRoute.Value with Source = 3}
                    else
                        selectedRoute.Set {selectedRoute.Value with Dest = 3}
            )
            .SelectLS(
                fun e ->
                    if e.Target.ParentElement.GetAttribute("name") = "image-map-source" then
                        selectedRoute.Set {selectedRoute.Value with Source = 0}
                    else
                        selectedRoute.Set {selectedRoute.Value with Dest = 0}
            )
            .SelectSF(
                fun e ->
                    if e.Target.ParentElement.GetAttribute("name") = "image-map-source" then
                        selectedRoute.Set {selectedRoute.Value with Source = 2}
                    else
                        selectedRoute.Set {selectedRoute.Value with Dest = 2}
            )
            .SelectBS(
                fun e ->
                    if e.Target.ParentElement.GetAttribute("name") = "image-map-source" then
                        selectedRoute.Set {selectedRoute.Value with Source = 5}
                    else
                        selectedRoute.Set {selectedRoute.Value with Dest = 5}
            )
            .SelectOS(
                fun e ->
                    if e.Target.ParentElement.GetAttribute("name") = "image-map-source" then
                        selectedRoute.Set {selectedRoute.Value with Source = 1}
                    else
                        selectedRoute.Set {selectedRoute.Value with Dest = 1}
            )
            .SelectCh(
                fun e ->
                    if e.Target.ParentElement.GetAttribute("name") = "image-map-source" then
                        selectedRoute.Set {selectedRoute.Value with Source = 4}
                    else
                        selectedRoute.Set {selectedRoute.Value with Dest = 4}
            )
            .Submit(
                fun e ->
                    async{
                        let sessionID = JavaScript.Cookies.Get("clms_sid").Value
                        let! callResult = Taxi.submitCall sessionID selectedRoute.Value.Source selectedRoute.Value.Dest
                        match callResult with
                            |CallResult.Success -> 
                                selectedRoute.Set {Source = -1; Dest = -1}
                                Feedback.giveFeedback false "Sikeres művelet!"
                            |CallResult.InvalidSession -> Feedback.giveFeedback true "Érvénytelen munkamenet. Jelentkezz be és ki újra!"
                            |CallResult.DatabaseError -> Feedback.giveFeedback true "Adatbázishiba. Értesítsd a (műszaki) igazgatót!"
                    } |> Async.Start
            )
            .Reset(
                fun e ->
                    selectedRoute.Set {Source = -1; Dest = -1}
            )
            .Doc()

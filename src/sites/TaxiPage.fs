namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open System.Collections.Generic

[<JavaScript>]
type TaxiRoute = {Source:int; Dest:int}

[<JavaScript>]
module TaxiPage =
    let selectedRoute = Var.Create {Source = -1; Dest = -1}
    let areaList = Var.Create <| Map.ofSeq []
    let updateAreaList() =
        async{
            let! list = Calls.getAreaList()
            match list with
                |Ok l ->
                    areaList.Set l
                |Error e ->
                    Feedback.giveFeedback true <| "Hiba a zónák lekérdezésekor: " + e
                    JavaScript.JS.Document.GetElementById("Submit").Remove()
        } |> Async.Start
    let getAreaName id =
       match Map.tryFind id areaList.Value with
        |None -> ""
        |Some s -> s
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
                                    let! isDouble = Calls.clientGetDPStatus()
                                    if price > 0 then JavaScript.JS.Document.GetElementById("Submit").RemoveAttribute("disabled")
                                    return (getAreaName r.Source + " - " + getAreaName r.Dest + ": " + string price + "$" + if isDouble then " (DUPLA)" else "")
                            } 
                )
            )
            .SourceList(
                Doc.BindSeqCached (
                    fun (kvp:KeyValuePair<int,string>) ->
                        SiteParts.TaxiTemplate.LocationItem()
                            .LocationID(string kvp.Key)
                            .LocationName(kvp.Value)
                            .Doc()
                ) areaList.View
            )
            .DestinationList(
                 Doc.BindSeqCached (
                    fun (kvp:KeyValuePair<int,string>) ->
                        SiteParts.TaxiTemplate.LocationItem()
                            .LocationID(string kvp.Key)
                            .LocationName(kvp.Value)
                            .Doc()
                ) areaList.View
            )
            .From(
                selectedRoute.Lens (fun r -> string r.Source) (fun r s -> {r with Source = int s})
            )
            .To(
                selectedRoute.Lens (fun r -> string r.Dest) (fun r s -> {r with Dest = int s})
            )
            .Submit(
                fun e ->
                    async{
                        let sessionID = JavaScript.Cookies.Get("clms_sid").Value
                        ActionDispatcher.RunAction Taxi.submitCall (sessionID,selectedRoute.Value.Source,selectedRoute.Value.Dest) None
                    } |> Async.Start
            )
            .Reset(
                fun e ->
                    selectedRoute.Set {Source = -1; Dest = -1}
                    JavaScript.JS.Window.Location.Replace "/"
            )
            .Doc()

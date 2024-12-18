namespace camblms.sites

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

open camblms.dto
open camblms.templating
open camblms.server.controller

[<JavaScript>]
module CarsAdmin =
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
    let mutable canEdit = false
    let tuningList = ListModel.FromSeq <| Map.ofList [ (0, "Gyári") ]
    let carList = ListModel.Create (fun (c: Car) -> c) []
    let memberList = ListModel.Create (fun (m: Member) -> m) []

    let selectedCar =
        Var.Create
            { Id = ""
              RegNum = ""
              CarType = ""
              ParkTicket = false
              ECU = 0
              GPS = false
              AirRide = false
              Engine = 0
              Brakes = 0
              Suspension = 0
              WeightReduction = 0
              Gearbox = 0
              Tyres = 0
              Turbo = 0
              KeyHolder = None
              WorkType = CarWorkType.Other }

    let unHideEdit () =
        JavaScript.JS.Document.GetElementById("edit").RemoveAttribute("style")

    let updateTuningList () =
        async {
            let! list = CarController.getTuningLevels ()

            match list with
            | Ok l -> l |> tuningList.Set
            | Error e -> Feedback.giveFeedback true <| "Hiba a tuningok lekérdezésekor: " + e
        }
        |> Async.Start

    let updateCarList () =
        async {
            let! list = CarController.getCars sessionID
            let! ce = PermissionController.doCheckPermission sessionID Permissions.CarAdmin
            canEdit <- ce

            if ce then
                unHideEdit ()

            match list with
            | Ok l -> l |> carList.Set
            | Error e -> Feedback.giveFeedback true <| "Hiba az autók lekérésekor: " + e
        }
        |> Async.Start

    let updateMemberList () =
        async {
            let! list = UserController.getUserList sessionID false false

            match list with
            | Ok l -> memberList.Set l
            | Error e -> Feedback.giveFeedback true <| "Hiba a tagok lekérésekor: " + e
        }
        |> Async.Start

    let renderTuningItem (t: System.Collections.Generic.KeyValuePair<int, string>) =
        SiteParts.CarTemplate
            .TuningItem()
            .TuningLevel(string t.Key)
            .TuningName(t.Value)
            .Doc()

    let getTuningName level =
        (query {
            for kvp in tuningList.Value do
                where (kvp.Key = level)
                exactlyOne
        })
            .Value

    let RenderPage () =
        updateTuningList ()
        updateMemberList ()
        updateCarList ()

        SiteParts
            .CarTemplate()
            .BrakeTuningList(tuningList.View |> Doc.BindSeqCached(renderTuningItem))
            .ECUTuningList(tuningList.View |> Doc.BindSeqCached(renderTuningItem))
            .EngineTuningList(tuningList.View |> Doc.BindSeqCached(renderTuningItem))
            .GearboxTuningList(tuningList.View |> Doc.BindSeqCached(renderTuningItem))
            .SuspensionTuningList(tuningList.View |> Doc.BindSeqCached(renderTuningItem))
            .TurboTuningList(tuningList.View |> Doc.BindSeqCached(renderTuningItem))
            .TyreTuningList(tuningList.View |> Doc.BindSeqCached(renderTuningItem))
            .WeightReductionTuningList(tuningList.View |> Doc.BindSeqCached(renderTuningItem))
            .MemberList1(
                memberList.View
                |> Doc.BindSeqCached(fun u ->
                    SiteParts.CarTemplate.KHListMember().UserID(string u.Id).Name(u.Name).Doc())
            )
            .NewID(selectedCar.Lens (fun c -> c.Id) (fun c id -> { c with Id = id }))
            .NewType(selectedCar.Lens (fun c -> c.CarType) (fun c t -> { c with CarType = t }))
            .NewRegNum(selectedCar.Lens (fun c -> c.RegNum) (fun c rn -> { c with RegNum = rn }))
            .NewKeyHolder1(
                selectedCar.Lens
                    (fun c ->
                        match c.KeyHolder with
                        | None -> "-1"
                        | Some m -> string m.Id)
                    (fun c idS ->
                        { c with
                            KeyHolder =
                                if idS = "-1" then
                                    None
                                else
                                    query {
                                        for u in memberList do
                                            where (u.Id = int idS)
                                            exactlyOne
                                    }
                                    |> Some })
            )
            .NewWorkType(
                selectedCar.Lens
                    (fun c -> int c.WorkType |> string)
                    (


                    fun c wt -> { c with WorkType = enum (int wt) })

            )
            .NewAirRide((selectedCar.Lens (fun c -> c.AirRide) (fun c ar -> { c with AirRide = ar })))
            .NewGPS(selectedCar.Lens (fun c -> c.GPS) (fun c gps -> { c with GPS = gps }))
            .NewTicket(selectedCar.Lens (fun c -> c.ParkTicket) (fun c pt -> { c with ParkTicket = pt }))
            .NewEngine(selectedCar.Lens (fun c -> string c.Engine) (fun c s -> { c with Engine = int s }))
            .NewECU(selectedCar.Lens (fun c -> string c.ECU) (fun c s -> { c with ECU = int s }))
            .NewBrakes(selectedCar.Lens (fun c -> string c.Brakes) (fun c s -> { c with Brakes = int s }))
            .NewSuspension(selectedCar.Lens (fun c -> string c.Suspension) (fun c s -> { c with Suspension = int s }))
            .NewTurbo(selectedCar.Lens (fun c -> string c.Turbo) (fun c s -> { c with Turbo = int s }))
            .NewTyres(selectedCar.Lens (fun c -> string c.Tyres) (fun c s -> { c with Tyres = int s }))
            .NewWeightReduction(
                selectedCar.Lens (fun c -> string c.WeightReduction) (fun c s -> { c with WeightReduction = int s })
            )
            .NewGearbox(selectedCar.Lens (fun c -> string c.Gearbox) (fun c s -> { c with Gearbox = int s }))
            .Confirm(fun _ ->
                if not canEdit then
                    ()
                else
                    async {
                        ActionDispatcher.RunAction
                            CarController.setCar
                            (sessionID, selectedCar.Value)
                            (Some(fun () ->
                                updateCarList ()
                                updateTuningList ()
                                updateMemberList ()))
                    }
                    |> Async.Start)
            .CarList(
                carList.View
                |> Doc.BindSeqCached(fun c ->
                    SiteParts.CarTemplate
                        .CarItem()
                        .Brakes(getTuningName c.Brakes)
                        .CarReg(c.RegNum)
                        .CarType(c.CarType)
                        .Engine(getTuningName c.Engine)
                        .ECU(getTuningName c.ECU)
                        .Gearbox(getTuningName c.Gearbox)
                        .Suspension(getTuningName c.Suspension)
                        .Turbo(getTuningName c.Turbo)
                        .Tyres(getTuningName c.Tyres)
                        .ID(c.Id)
                        .WeightReduction(getTuningName c.WeightReduction)
                        .OldAirRide(c.AirRide)
                        .OldGPS(c.GPS)
                        .OldTicket(c.ParkTicket)
                        .Key1(
                            match c.KeyHolder with
                            | None -> ""
                            | Some u -> u.Name
                        )
                        .WorkType(
                            match c.WorkType with
                            | CarWorkType.Other -> "Egyéb"
                            | CarWorkType.Taxi -> "Taxi"
                            | CarWorkType.Tow -> "Vontató"
                            | _ -> "Ismeretlen"
                        )
                        .EditButtonPlaceholder(
                            if not canEdit then
                                Doc.Empty
                            else
                                SiteParts.CarTemplate
                                    .EditButtonTemplate()
                                    .Edit(fun _ -> selectedCar.Set c)
                                    .Doc()
                        )
                        .DeleteButtonPlaceholder(
                            if not canEdit then
                                Doc.Empty
                            else
                                SiteParts.CarTemplate
                                    .DeleteButtonTemplate()
                                    .Delete(fun _ ->
                                        async {
                                            ActionDispatcher.RunAction
                                                CarController.deleteCar
                                                (sessionID, c)
                                                (Some(fun () ->
                                                    updateCarList ()
                                                    updateMemberList ()
                                                    updateTuningList ()))

                                        }
                                        |> Async.Start)
                                    .Doc()
                        )
                        .Doc())
            )
            .Doc()

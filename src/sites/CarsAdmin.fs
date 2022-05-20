namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module CarsAdmin =
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
    let tuningList = ListModel.FromSeq <| Map.ofList [(0,"Gyári")]
    let carList = ListModel.FromSeq [{Id = "L04D1NG";RegNum="HAW-411";CarType="Fiat Panda II";
                                                            ParkTicket=true;ECU=0;GPS=false;
                                                            AirRide = false;Engine=0;Brakes=0;
                                                            Suspension=0;WeightReduction=0;Gearbox=0;
                                                            Tyres=0;Turbo=0;KeyHolder1=None;
                                                            KeyHolder2=None}]
    let memberList = ListModel.FromSeq [{Member.Id=1;Name="Loa Ding";Role=0;Email="@";AccountID=00000}]
    let selectedCar = Var.Create {Id = "";RegNum="";CarType="";
                                                            ParkTicket=false;ECU=0;GPS=false;
                                                            AirRide = false;Engine=0;Brakes=0;
                                                            Suspension=0;WeightReduction=0;Gearbox=0;
                                                            Tyres=0;Turbo=0;KeyHolder1=None;
                                                            KeyHolder2=None}
    let updateTuningList =
        async{
            let! list = Cars.getTuningLevels
            tuningList.Set list
        }
    let updateCarList =
        async{
            let! list = Cars.doGetCars sessionID
            carList.Set list
        }
    let updateMemberList =
        async{
            let! list = UserCallable.doGetUserList sessionID false
            memberList.Set list
        }
    let renderTuningItem (t:System.Collections.Generic.KeyValuePair<int,string>) =
        SiteParts.CarTemplate.TuningItem()
            .TuningLevel(string t.Key)
            .TuningName(t.Value)
            .Doc()
    let getTuningName level =
        (query{
            for kvp in tuningList.Value do
            where(kvp.Key = level)
            exactlyOne
        }).Value
    let RenderPage =
        updateTuningList |> Async.Start
        updateMemberList |> Async.Start
        updateCarList |> Async.Start
        SiteParts.CarTemplate()
            .BrakeTuningList(
                tuningList.View |> Doc.BindSeqCached (renderTuningItem)
            )
            .ECUTuningList(
                tuningList.View |> Doc.BindSeqCached (renderTuningItem)
            )
            .EngineTuningList(tuningList.View |> Doc.BindSeqCached (renderTuningItem))
            .GearboxTuningList(tuningList.View |> Doc.BindSeqCached (renderTuningItem))
            .SuspensionTuningList(tuningList.View |> Doc.BindSeqCached (renderTuningItem))
            .TurboTuningList(tuningList.View |> Doc.BindSeqCached (renderTuningItem))
            .TyreTuningList(tuningList.View |> Doc.BindSeqCached (renderTuningItem))
            .WeightReductionTuningList(tuningList.View |> Doc.BindSeqCached (renderTuningItem))
            .MemberList1(
                memberList.View |> Doc.BindSeqCached(
                    fun u ->
                        SiteParts.CarTemplate.KHListMember()
                            .UserID(string u.Id)
                            .Name(u.Name)
                            .Doc()
                )
            )
            .MemberList2(
                 memberList.View |> Doc.BindSeqCached(
                    fun u ->
                        SiteParts.CarTemplate.KHListMember()
                            .UserID(string u.Id)
                            .Name(u.Name)
                            .Doc()
                )
            )
            .NewID(selectedCar.LensAuto (fun c -> c.Id))
            .NewType(selectedCar.LensAuto (fun c -> c.CarType))
            .NewRegNum(selectedCar.LensAuto(fun c -> c.RegNum))
            .NewKeyHolder1(selectedCar.Lens(
                fun c ->
                    match c.KeyHolder1 with
                        |None -> "-1"
                        |Some m -> string m.Id
                )
                (
                    fun c idS ->
                        {
                            c with KeyHolder1 = 
                                    if idS = "-1" then None 
                                    else
                                    query{
                                        for u in memberList do
                                        where(u.Id = int idS)
                                        exactlyOne
                                    } |> Some
                        } 
                )
            )
            .NewKeyHolder2(selectedCar.Lens(
                fun c ->
                    match c.KeyHolder2 with
                        |None -> "-1"
                        |Some m -> string m.Name 
                )
                (
                    fun c idS ->
                        {
                            c with KeyHolder2 = 
                                    if idS = "-1" then None 
                                    else
                                    query{
                                        for u in memberList do
                                        where(u.Id = int idS)
                                        exactlyOne
                                    } |> Some
                        } 
                )
                )
            .NewAirRide(selectedCar.LensAuto (fun c -> c.AirRide))
            .NewGPS(selectedCar.LensAuto(fun c -> c.GPS))
            .NewTicket(selectedCar.LensAuto (fun c -> c.ParkTicket))
            .NewEngine(selectedCar.Lens(fun c -> string c.Engine) (fun c s -> {c with Engine = int s}))
            .NewECU(selectedCar.Lens(fun c -> string c.ECU) (fun c s -> {c with ECU = int s}))
            .NewBrakes(selectedCar.Lens(fun c -> string c.Brakes) (fun c s -> {c with Brakes = int s}))
            .NewSuspension(selectedCar.Lens(fun c -> string c.Suspension) (fun c s -> {c with Suspension = int s}))
            .NewTurbo(selectedCar.Lens(fun c -> string c.Turbo) (fun c s -> {c with Turbo = int s}))
            .NewTyres(selectedCar.Lens(fun c -> string c.Tyres) (fun c s -> {c with Tyres = int s}))
            .NewWeightReduction(selectedCar.Lens(fun c -> string c.WeightReduction) (fun c s -> {c with WeightReduction = int s}))
            .NewGearbox(selectedCar.Lens(fun c -> string c.Gearbox) (fun c s -> {c with Gearbox = int s}))
            .Confirm(
                fun _ ->
                    async{
                        let! result = Cars.doSetCar sessionID selectedCar.Value
                        return! updateCarList
                    } |> Async.Start
            )
            .CarList(
                carList.View |> Doc.BindSeqCached(
                    fun c ->
                        SiteParts.CarTemplate.CarItem()
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
                                match c.KeyHolder1 with
                                    |None -> ""
                                    |Some u -> u.Name 
                            )
                            .Key2(
                                match c.KeyHolder2 with
                                    |None -> ""
                                    |Some u -> u.Name 
                            )
                            .Edit(
                                fun _ ->
                                    selectedCar.Set c
                            )
                            .Doc()
                )
            )
            .Doc()
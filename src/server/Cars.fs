namespace camblms

open WebSharper

[<JavaScript>]
type CarWorkType = 
    |Other = -1
    |Taxi = 0
    |Tow = 1

[<JavaScript>]
type Car =
    { Id: string
      CarType: string
      RegNum: string
      AirRide: bool
      GPS: bool
      ParkTicket: bool
      Engine: int
      ECU: int
      Brakes: int
      Suspension: int
      WeightReduction: int
      Gearbox: int
      Turbo: int
      Tyres: int
      KeyHolder: Member option
      WorkType: CarWorkType}

module Cars =
    [<Rpc>]
    let getTuningLevels () =
        async {
            try
                let db = Database.getDataContext ()
                return
                    Ok(query {
                        for tuning in db.Camblogistics.tuninglevels do
                            select (tuning.Level, tuning.Name)
                    }
                    |> Map.ofSeq)
            with
            | e -> return Error e.Message
        }

    [<Rpc>]
    let getCars sid =
        async {
            if not (Permission.checkPermission sid Permissions.ViewCars) then
                return Error "Nincs jogosultságod az autók megtekintéséhez!"
            else
                try
                    let db = Database.getDataContext ()
                    return
                        Ok(query {
                            for car in db.Camblogistics.cars do
                                select (
                                    { Id = car.Id
                                      RegNum = car.RegNum
                                      CarType = car.Type
                                      KeyHolder =
                                        if car.KeyHolder1.IsSome then
                                            User.getUserByID car.KeyHolder1.Value
                                        else
                                            None
                                      AirRide = car.AirRide = (sbyte 1)
                                      ParkTicket = car.Ticket = (sbyte 1)
                                      GPS = car.Gps = (sbyte 1)
                                      Engine = car.Engine
                                      ECU = car.Ecu
                                      Brakes = car.Brakes
                                      Suspension = car.Suspension
                                      Gearbox = car.Gearbox
                                      Tyres = car.Tyres
                                      Turbo = car.Turbo
                                      WeightReduction = car.WeightReduction
                                      WorkType = LanguagePrimitives.EnumOfValue car.WorkType
                                     }
                                )
                        }
                        |> Seq.toList
                        |> List.sortBy (fun c -> c.RegNum))
                with
                | e -> return Error e.Message
        }

    let getCarsOfKeyHolder sid =
        let user = User.getUserFromSID sid
        match user with
        | None -> Error "Hiba a vezetett autók lekérése közben: nem létező felhasználó!"
        | Some u ->
            try
                let db = Database.getDataContext ()
                Ok(query {
                    for car in db.Camblogistics.cars do
                        where (
                            car.KeyHolder1 = Some u.Id
                        )

                        select (car.RegNum)
                }
                |> Seq.toList)
            with
            | e -> Error e.Message

    [<Rpc>]
    let setCar(sid,car) =
        async {
            try
                if not (Permission.checkPermission sid Permissions.CarAdmin) then
                    return ActionResult.InsufficientPermissions
                else
                    let db = Database.getDataContext ()

                    let existing =
                        query {
                            for c in db.Camblogistics.cars do
                                where (c.Id = car.Id)
                                select c
                        }
                        |> Seq.toList

                    let newCar =
                        if List.isEmpty existing then
                            db.Camblogistics.cars.Create()
                        else
                            List.item 0 existing

                    if List.isEmpty existing then
                        newCar.Id <- car.Id

                    newCar.Type <- car.CarType
                    newCar.RegNum <- car.RegNum
                    newCar.KeyHolder1 <-
                        if car.KeyHolder.IsSome then
                            Some car.KeyHolder.Value.Id
                        else
                            None
                    newCar.AirRide <- if car.AirRide then sbyte 1 else sbyte 0
                    newCar.Ticket <-
                        if car.ParkTicket then
                            sbyte 1
                        else
                            sbyte 0
                    newCar.Gps <- if car.GPS then sbyte 1 else sbyte 0
                    newCar.Ecu <- car.ECU
                    newCar.Engine <- car.Engine
                    newCar.Brakes <- car.Brakes
                    newCar.Gearbox <- car.Gearbox
                    newCar.Suspension <- car.Suspension
                    newCar.Turbo <- car.Turbo
                    newCar.Tyres <- car.Tyres
                    newCar.WeightReduction <- car.WeightReduction
                    newCar.WorkType <- LanguagePrimitives.EnumToValue car.WorkType
                    db.SubmitUpdates()
                    return ActionResult.Success
            with
            | e -> return OtherError e.Message
        }
    [<Rpc>]
    let delCar (sid,car) =
        async{
            try
                if not (Permission.checkPermission sid Permissions.CarAdmin) then
                    return ActionResult.InsufficientPermissions
                else
                    let db = Database.getDataContext ()
                    let dbCar =
                        query{
                            for c in db.Camblogistics.cars do
                                where(c.Id = car.Id)
                                exactlyOne
                        }
                    dbCar.Delete()
                    db.SubmitUpdates()
                    return ActionResult.Success
            with
            | e -> return OtherError e.Message
        }


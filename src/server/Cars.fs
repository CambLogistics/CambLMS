namespace camblms

open WebSharper

[<JavaScript>]
type Car = {Id:string;CarType: string;RegNum: string;
            AirRide: bool;GPS:bool;ParkTicket: bool;
            Engine: int;ECU: int;Brakes: int;
            Suspension: int;WeightReduction: int;Gearbox: int
            Turbo: int; Tyres:int
            KeyHolder1: Member option;KeyHolder2: Member option}

module Cars =
    [<Rpc>]
    let getTuningLevels() =
        async{
        try
            let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
            return query{
                for tuning in db.Camblogistics.tuninglevels do
                select (tuning.Level,tuning.Name)
            } |> Map.ofSeq
        with
            _ -> return Map.ofList []
        }
    let getCars sid =
        if not (Permission.checkPermission sid Permissions.ViewCars) |> not then []
        else
        try
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        query{
            for car in db.Camblogistics.cars do
                select(
                    {
                        Id = car.Id
                        RegNum = car.RegNum
                        CarType = car.Type
                        KeyHolder1 = 
                            if car.KeyHolder1.IsSome then 
                                User.getUserByID car.KeyHolder1.Value
                                else None
                        KeyHolder2 = 
                            if car.KeyHolder2.IsSome then 
                                User.getUserByID car.KeyHolder2.Value 
                                else None
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
                    }
            )
        } |> Seq.toList |> List.sortBy (fun c -> c.RegNum)
        with
            _ -> []
    let getCarsOfKeyHolder sid =
        let user = User.getUserFromSID sid
        match user with
            |None -> []
            |Some u ->
                try
                let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
                query{
                    for car in db.Camblogistics.cars do
                    where(car.KeyHolder1 = Some u.Id || car.KeyHolder2 = Some u.Id)
                    select(
                        car.RegNum
                    )
                } |> Seq.toList
                with
                    _ -> []
    let setCar sid car =
        try
        if not (Permission.checkPermission sid Permissions.CarAdmin) then ()
        else
            let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
            let existing =
                query{
                    for c in db.Camblogistics.cars do
                    where(c.Id = car.Id)
                    select c          
                } |> Seq.toList
            let newCar = 
                if List.isEmpty existing then db.Camblogistics.cars.Create() else List.item 0 existing
            if List.isEmpty existing then newCar.Id <- car.Id
            newCar.Type <- car.CarType
            newCar.RegNum <- car.RegNum
            newCar.KeyHolder1 <- if car.KeyHolder1.IsSome then Some car.KeyHolder1.Value.Id else None
            newCar.KeyHolder2 <- if car.KeyHolder2.IsSome then Some car.KeyHolder2.Value.Id else None
            newCar.AirRide <- if car.AirRide then sbyte 1 else sbyte 0
            newCar.Ticket <- if car.ParkTicket then sbyte 1 else sbyte 0
            newCar.Gps <- if car.GPS then sbyte 1 else sbyte 0
            newCar.Ecu <- car.ECU
            newCar.Engine <- car.Engine
            newCar.Brakes <- car.Brakes
            newCar.Gearbox <- car.Gearbox
            newCar.Suspension <- car.Suspension
            newCar.Turbo <- car.Turbo
            newCar.Tyres <- car.Tyres
            newCar.WeightReduction <- car.WeightReduction
            db.SubmitUpdates()
        with
            _ -> ()
    [<Rpc>]
    let doGetCars sid =
        async{
            return getCars sid
        }
    [<Rpc>]
    let doSetCar sid car =
        async{
            return setCar sid car
        }

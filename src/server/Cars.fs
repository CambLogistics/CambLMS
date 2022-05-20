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
    let getTuningLevels =
        async{
        try
            let db = Database.SqlConnection.GetDataContext()
            return query{
                for tuning in db.Camblogistics.TuningLevels do
                select (tuning.Level,tuning.Name)
            } |> Map.ofSeq
        with
            _ -> return Map.ofList []
        }
    let getCars sid =
        if not (User.verifyAdmin sid) then []
        else
        try
        let db = Database.SqlConnection.GetDataContext()
        query{
            for car in db.Camblogistics.Cars do
            join user1 in db.Camblogistics.Users on (car.KeyHolder1 = Some user1.Id)
            join user2 in db.Camblogistics.Users on (car.KeyHolder2 = Some user2.Id)
            select(
                {
                    Id = car.Id
                    RegNum = car.RegNum
                    CarType = car.Type
                    KeyHolder1 = 
                        if car.KeyHolder1.IsSome then 
                            Some {Id = user1.Id;Name = user1.Name;Role = user1.Role;AccountID = user1.AccountId;Email = user1.Email} 
                            else None
                    KeyHolder2 = 
                        if car.KeyHolder2.IsSome then 
                            Some {Id = user2.Id;Name = user2.Name;Role = user2.Role;AccountID = user2.AccountId;Email = user2.Email} 
                            else None
                    AirRide = car.AirRide = (sbyte 1)
                    ParkTicket = car.Ticket = (sbyte 1) 
                    GPS = car.AirRide = (sbyte 1)
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
        } |> Seq.toList
        with
            _ -> []
    let getCarsOfKeyHolder sid =
        let user = User.getUserFromSID sid
        match user with
            |None -> []
            |Some u ->
                try
                let db = Database.SqlConnection.GetDataContext()
                query{
                    for car in db.Camblogistics.Cars do
                    where(car.KeyHolder1 = Some u.Id || car.KeyHolder2 = Some u.Id)
                    select(
                        car.RegNum
                    )
                } |> Seq.toList
                with
                    _ -> []
    let setCar sid car =
        if not (User.verifyAdmin sid) then ()
        else
        try
            let db = Database.SqlConnection.GetDataContext()
            let existing =
                query{
                    for c in db.Camblogistics.Cars do
                    where(c.Id = car.Id)
                    select c          
                } |> Seq.toList
            let newCar = 
                if List.isEmpty existing then db.Camblogistics.Cars.Create() else List.item 0 existing
            newCar.Id <- car.Id
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
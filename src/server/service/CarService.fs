namespace camblms.server.service

open FSharp.Data.Sql

open camblms.dto
open camblms.server.database

module CarService =
    let getTuningLevelsFromDb () =
        let db = Database.getDataContext ()

        query {
            for tuning in db.Camblogistics.tuninglevels do
                select (tuning.Level, tuning.Name)
        }
        |> Map.ofSeq

    let getCarsFromDb () =
        let db = Database.getDataContext ()

        query {
            for car in db.Camblogistics.cars do
                select (
                    { Id = car.Id
                      RegNum = car.RegNum
                      CarType = car.Type
                      KeyHolder =
                        if car.KeyHolder1.IsSome then
                            UserService.getUserByID car.KeyHolder1.Value
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
                      WorkType = LanguagePrimitives.EnumOfValue car.WorkType }
                )
        }
        |> Seq.toList
        |> List.sortBy (fun c -> c.RegNum)

    let getCarCountByWorkType workType =
        let db = Database.getDataContext ()
        let wt = LanguagePrimitives.EnumToValue workType

        query {
            for c in db.Camblogistics.cars do
                where (c.WorkType = wt)
                count
        }

    let getOccupiedCarCountByWorkType workType =
        let db = Database.getDataContext ()
        let wt = LanguagePrimitives.EnumToValue workType

        query {
            for c in db.Camblogistics.cars do
                where (c.WorkType = wt && not (c.KeyHolder1 = None))
                count
        }

    let getCarsOfKeyHolder (user: Member) =
        let db = Database.getDataContext ()

        query {
            for car in db.Camblogistics.cars do
                where (car.KeyHolder1 = Some user.Id)
                select (car.RegNum)
        }
        |> Seq.toList

    let getCarEntityForEdit id =
        let db = Database.getDataContext ()

        let existing =
            query {
                for c in db.Camblogistics.cars do
                    where (c.Id = id)
                    select c
            }

        let newCar =
            if Seq.isEmpty existing then
                db.Camblogistics.cars.Create()
            else
                Seq.item 0 existing

        newCar.Id <- id
        newCar

    let submitCarToDatabase car =
        let db = Database.getDataContext ()
        let newCar = getCarEntityForEdit car.Id
        newCar.Type <- car.CarType
        newCar.RegNum <- car.RegNum

        newCar.KeyHolder1 <-
            if car.KeyHolder.IsSome then
                Some car.KeyHolder.Value.Id
            else
                None

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
        newCar.WorkType <- LanguagePrimitives.EnumToValue car.WorkType
        db.SubmitUpdates()

    let deleteCar car =
        let db = Database.getDataContext ()

        (query {
            for c in db.Camblogistics.cars do
                where (c.Id = car.Id)
                exactlyOne
        })
            .Delete()

        db.SubmitUpdates()

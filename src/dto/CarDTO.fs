namespace camblms.dto

open WebSharper

[<JavaScript>]
type CarWorkType =
    | Other = -1
    | Taxi = 0
    | Tow = 1

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
      WorkType: CarWorkType }

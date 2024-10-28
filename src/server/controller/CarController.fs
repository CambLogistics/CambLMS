namespace camblms.server.controller

open WebSharper

open camblms.server.service
open camblms.dto

module CarController =
    [<Rpc>]
    let getCars sid =
        async {
            try
                let hasPermission = PermissionService.checkPermission sid Permissions.ViewCars

                if not hasPermission then
                    return Error "Nincs jogosultságod az autók megtekintéséhez!"
                else
                    return Ok <| CarService.getCarsFromDb ()
            with e ->
                return Error e.Message
        }

    [<Rpc>]
    let setCar (sid, car) =
        async {
            try
                let hasPermission = PermissionService.checkPermission sid Permissions.ViewCars

                if not hasPermission then
                    return ActionResult.InsufficientPermissions
                else
                    do CarService.submitCarToDatabase car
                    return ActionResult.Success
            with e ->
                return OtherError e.Message
        }

    [<Rpc>]
    let getTuningLevels () =
        async {
            try
                return Ok <| CarService.getTuningLevelsFromDb ()
            with e ->
                return Error e.Message
        }

    [<Rpc>]
    let getCarCountByWorkType sid (workType: CarWorkType) =
        async {
            let hasPermission = PermissionService.checkPermission sid Permissions.ViewCars

            if not hasPermission then
                return Error "Nincs jogosultságod az autók megtekintéséhez!"
            else
                return Ok(CarService.getCarCountByWorkType workType, CarService.getOccupiedCarCountByWorkType workType)
        }

    [<Rpc>]
    let deleteCar (sid, car) =
        async {
            try
                let hasPermission = PermissionService.checkPermission sid Permissions.CarAdmin

                if not hasPermission then
                    return ActionResult.InsufficientPermissions
                else
                    do CarService.deleteCar car
                    return ActionResult.Success
            with e ->
                return OtherError e.Message
        }

    [<Rpc>]
    let getCarsOfKeyHolder sid =
        async {
            try
                let user = UserService.getUserFromSID sid
                return Some <| CarService.getCarsOfKeyHolder user.Value
            with _ ->
                return None
        }

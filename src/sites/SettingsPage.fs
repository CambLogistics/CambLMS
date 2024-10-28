namespace camblms.sites

open WebSharper
open WebSharper.UI

open camblms.dto
open camblms.templating
open camblms.server.controller

[<JavaScript>]
module SettingsPage =
    let user =
        Var.Create
            { Id = 0
              Role = { Level = 0; Name = "" }
              Name = ""
              AccountID = 0
              Email = "" }

    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
    let callsOfUser = Var.Create []
    let carsOfUser = Var.Create []

    let getUser () =
        async {
            let! userProfile = UserController.getUserProfile sessionID
            user.Set userProfile.Value
        }
        |> Async.Start

    let getCarsOfUser () =
        async {
            let! carList = CarController.getCarsOfKeyHolder sessionID

            match carList with
            | Some c -> carsOfUser.Set c
            | None -> Feedback.giveFeedback true <| "Hiba az autólista lekérésekor!"
        }
        |> Async.Start

    let getCallsOfUser () =
        async {
            let! callList = CallsController.getCallsOfUser sessionID

            match callList with
            | Some c -> callsOfUser.Set c
            | None -> Feedback.giveFeedback true <| "Hiba az hívások lekérésekor!"
        }
        |> Async.Start

    let RenderPage () =
        getUser ()
        getCarsOfUser ()
        getCallsOfUser ()

        SiteParts
            .SettingsTemplate()
            .AccID(string user.V.AccountID)
            .Name(user.V.Name)
            .Cars(List.fold (fun s rn -> (s + " " + rn)) "" carsOfUser.V)
            .Rank(user.V.Role.Name)
            .MoneySum((callsOfUser.V |> List.sumBy (fun c -> c.Price) |> string) + " $")
            .TwoWeekMoney(
                (callsOfUser.V
                 |> List.filter (fun c -> c.ThisWeek || c.PreviousWeek)
                 |> List.sumBy (fun c -> c.Price)
                 |> string)
                + " $"
            )
            .TaxiSum(
                callsOfUser.V
                |> List.filter (fun c -> c.Type = CallType.Taxi)
                |> List.length
                |> string
            )
            .TowSum(
                callsOfUser.V
                |> List.filter (fun c -> c.Type = CallType.Towing)
                |> List.length
                |> string
            )
            .Contract("#") //Not implemented
            .Doc()

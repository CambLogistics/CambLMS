namespace camblms.sites

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

open camblms.dto
open camblms.templating
open camblms.server.controller

[<JavaScript>]
module Information =
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
    let callPercentage = Var.Create 0

    let user =
        Var.Create
            { Id = -1
              Role = { Level = 0; Name = "" }
              Name = ""
              AccountID = 0
              Email = "" }

    let cars = Var.Create []
    let calls = Var.Create []

    let recentCalls = Var.Create []

    let getUser () =
        async {
            let! userProfile = UserController.getUserProfile sessionID
            user.Set userProfile.Value
        }
        |> Async.Start

    let getCallPercentage () =
        async {
            let! callPc = CallsController.getWeeklyCallPercentage sessionID

            match callPc with
            | Ok c -> callPercentage.Set c
            | Error _ -> Feedback.giveFeedback true <| "Hiba a heti hívásszazalék lekérésekor!"
        }
        |> Async.Start

    let getCarsOfUser () =
        async {
            let! carList = CarController.getCarsOfKeyHolder sessionID

            match carList with
            | Some c -> cars.Set c
            | None -> Feedback.giveFeedback true <| "Hiba az autólista lekérésekor!"
        }
        |> Async.Start

    let getCallsOfUser () =
        async {
            let! callList = CallsController.getCallsOfUser sessionID

            match callList with
            | Some c ->
                do calls.Set c

                c
                |> List.sortByDescending (fun x -> x.Date)
                |> List.take (if List.length c >= 5 then 5 else List.length c)
                |> recentCalls.Set
            | None -> Feedback.giveFeedback true <| "Hiba az hívások lekérésekor!"
        }
        |> Async.Start

    let RenderPage () =
        try
            getUser ()
            getCallPercentage ()
            getCarsOfUser ()
            getCallsOfUser ()

            SiteParts
                .InfoTemplate()
                .Rank(user.V.Role.Name)
                .Cars(List.fold (fun s rn -> (s + " " + rn)) "" cars.V)
                .MoneySum((calls.V |> List.sumBy (fun c -> c.Price) |> string))
                .CallSum(List.length calls.V |> string)
                .WeeklyCallWidth(string <| (if callPercentage.V <= 100 then callPercentage.V else 100))
                .WeeklyCallPercentage(string <| callPercentage.V)
                .RecentCalls(
                    recentCalls.View
                    |> Doc.BindSeqCached(fun c ->
                        SiteParts.InfoTemplate
                            .CallItem()
                            .Date(
                                sprintf
                                    "%04d-%02d-%02d %02d:%02d"
                                    c.Date.Year
                                    c.Date.Month
                                    c.Date.Day
                                    c.Date.Hour
                                    c.Date.Minute
                            )
                            .Type(
                                match c.Type with
                                | CallType.Taxi -> "Taxi"
                                | CallType.Towing -> "Vonti"
                                | CallType.Delivery -> "Fuvar (ELAVULT!)"
                                | _ -> "Ismeretlen"
                            )
                            .Price(sprintf "%d" c.Price)
                            .Doc())
                )
                .Doc()
        with e ->
            SiteParts
                .NotFoundTemplate()
                .ErrorMessage("Hiba az információs oldal betöltése közben! Értesítsd a (műszaki) igazgatót!")
                .Doc()

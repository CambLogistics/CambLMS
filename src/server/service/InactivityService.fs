namespace camblms.server.service

open System
open FSharp.Data.Sql

open camblms.dto
open camblms.server.database

module InactivityService =
    let getActiveStatus (user: Member) =
        let db = Database.getDataContext ()

        query {
            for ir in db.Camblogistics.inactivity do
                where (
                    ir.Userid = user.Id
                    && ir.Beginning < DateTime.Now
                    && ir.Ending > DateTime.Now
                    && ir.Accepted = (sbyte 1)
                )

                count
        } = 0

    let getUserStatusList () =
        let users = UserService.getUserList false false

        let (statusList, userList) =
            users |> List.mapFold (fun l u -> (getActiveStatus u, u :: l)) []

        List.rev userList
        |> List.zip statusList
        |> List.map (fun (s, u) ->
            { UserName = u.Name
              UserID = u.Id
              Status = s })

    let hasOverlappingRequest (user: Member) startDate endDate =
        let db = Database.getDataContext ()

        query {
            for ir in db.Camblogistics.inactivity do
                where (
                    ((ir.Beginning < startDate && ir.Ending > startDate)
                     || (ir.Beginning > startDate && ir.Beginning < endDate))
                    && ir.Userid = user.Id
                    && (ir.Pending = (sbyte 1) || ir.Accepted = (sbyte 1))
                )

                count
        } > 0

    let requestInactivity (user: Member) startDate endDate reason =
        let db = Database.getDataContext ()

        let existing =
            query {
                for ir in db.Camblogistics.inactivity do
                    where (ir.Beginning = startDate && ir.Ending = endDate && user.Id = ir.Userid)
                    select ir
            }

        let newRequest =
            if Seq.length existing > 0 then
                Seq.item 0 existing
            else
                db.Camblogistics.inactivity.Create()

        newRequest.Accepted <- (sbyte 0)
        newRequest.Pending <- (sbyte 1)
        newRequest.Userid <- user.Id
        newRequest.Beginning <- startDate
        newRequest.Ending <- endDate
        newRequest.Reason <- reason
        db.SubmitUpdates()

    let decideRequest (request: InactivityRequest) decision =
        let db = Database.getDataContext ()

        let requestEntity =
            query {
                for r in db.Camblogistics.inactivity do
                    where (r.Userid = request.UserID && r.Pending = (sbyte 1))
                    select r
            }
            |> Seq.filter (fun r ->
                r.Beginning.Subtract(request.From.ToLocalTime()).TotalMinutes < 1.0
                && r.Ending.Subtract(request.To.ToLocalTime()).TotalMinutes < 1.0)
            |> Seq.item 0

        requestEntity.Accepted <- if decision then sbyte 1 else sbyte 0
        requestEntity.Pending <- sbyte 0
        db.SubmitUpdates()

    let getPendingRequests () =
        let db = Database.getDataContext ()

        query {
            for r in db.Camblogistics.inactivity do
                join u in db.Camblogistics.users on (r.Userid = u.Id)
                where (r.Pending = (sbyte 1))

                select (
                    { UserName = u.Name
                      UserID = u.Id
                      From = r.Beginning
                      To = r.Ending
                      Reason = r.Reason }
                )
        }
        |> Seq.toList

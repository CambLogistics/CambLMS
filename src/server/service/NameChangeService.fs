namespace camblms.server.service

open FSharp.Data.Sql

open camblms.dto
open camblms.server.database

module NameChangeService =
    let hasPendingRequest (user: Member) =
        let db = Database.getDataContext ()

        query {
            for x in db.Camblogistics.namechanges do
                where (x.UserId = user.Id && x.Pending = (sbyte 1))
                count
        } > 0

    let requestNameChange (user: Member) newName =
        let db = Database.getDataContext ()
        let newChange = db.Camblogistics.namechanges.Create()
        newChange.Approved <- (sbyte 0)
        newChange.NewName <- newName
        newChange.Pending <- sbyte 1
        newChange.UserId <- user.Id
        db.SubmitUpdates()

    let decideRequest userID decision =
        let db = Database.getDataContext ()

        let nc =
            query {
                for x in db.Camblogistics.namechanges do
                    where (x.UserId = userID && x.Pending = (sbyte 1))
                    exactlyOne
            }

        nc.Pending <- (sbyte 0)

        if decision then
            nc.Approved <- (sbyte 1)

            let user =
                query {
                    for u in db.Camblogistics.users do
                        where (u.Id = userID)
                        exactlyOne
                }

            user.Name <- nc.NewName
        else
            nc.Approved <- (sbyte 0)

        db.SubmitUpdates()

    let getPendingChanges () =
        let db = Database.getDataContext ()

        query {
            for x in db.Camblogistics.namechanges do
                join u in db.Camblogistics.users on (x.UserId = u.Id)
                where (x.Pending = (sbyte 1))

                select (
                    { UserID = x.UserId
                      OldName = u.Name
                      NewName = x.NewName }
                )
        }
        |> Seq.toList

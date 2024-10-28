namespace camblms.server.service

open FSharp.Data.Sql

open camblms.dto
open camblms.server.database

module BlacklistService =
    let getBlacklist () =
        let db = Database.getDataContext ()

        query {
            for i in db.Camblogistics.blacklist do
                select
                    { UserName = i.Name
                      Role = i.RoleId
                      AccountID = i.AccountId
                      Reason = i.Reason
                      Comeback = (i.CanReturn = sbyte 1) }
        }
        |> Seq.toList

    let setBlacklistItem bli =
        let db = Database.getDataContext ()

        let existing =
            query {
                for i in db.Camblogistics.blacklist do
                    where (i.AccountId = bli.AccountID)
                    select i
            }

        let newItem =
            if Seq.isEmpty existing then
                db.Camblogistics.blacklist.Create()
            else
                (Seq.item 0 existing)

        if Seq.isEmpty existing then
            newItem.AccountId <- bli.AccountID

        newItem.CanReturn <- if bli.Comeback then sbyte 1 else sbyte 0
        newItem.Reason <- bli.Reason
        newItem.Name <- bli.UserName
        newItem.RoleId <- bli.Role
        db.SubmitUpdates()

    let deleteBlacklistItem bli =
        let db = Database.getDataContext ()

        let existing =
            query {
                for i in db.Camblogistics.blacklist do
                    where (i.AccountId = bli.AccountID)
                    exactlyOne
            }

        do existing.Delete()
        db.SubmitUpdates()

namespace camblms.server.service

open FSharp.Data.Sql

open camblms.dto
open camblms.server.database

module ServiceFeeService =
    let getPendingFees () =
        let db = Database.getDataContext ()

        query {
            for fee in db.Camblogistics.servicefees do
                where (fee.Paid = (sbyte 0))
                join u in db.Camblogistics.users on (fee.UserId = u.Id)

                select (
                    { ID = fee.Id
                      Username = u.Name
                      Amount = fee.Amount }
                )
        }
        |> Seq.toList

    let submitPendingFee userid amount =
        let db = Database.getDataContext ()
        let newFee = db.Camblogistics.servicefees.Create()
        newFee.Amount <- amount
        newFee.UserId <- userid
        newFee.Date <- System.DateTime.Now
        newFee.Paid <- (sbyte 0)
        db.SubmitUpdates()

    let payFee id =
        let db = Database.getDataContext ()

        let fee =
            query {
                for f in db.Camblogistics.servicefees do
                    where (f.Id = id)
                    exactlyOne
            }

        fee.Paid <- (sbyte 1)
        db.SubmitUpdates()

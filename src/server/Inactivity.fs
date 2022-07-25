namespace camblms
open WebSharper
open System

[<JavaScript>]
type InactivityRequest = {
    UserName: string
    UserID: int
    From: DateTime
    To: DateTime
    Reason: string
}

[<JavaScript>]
type InactivityRequestSuccess =
    |Success
    |Overlap
    |InvalidSession
    |DatabaseError

module Inactivity =
    [<Rpc>]
    let getActiveStatus (user:Member) =
        async{
            let db = Database.getDataContext()
            return (query{
                for ir in db.Camblogistics.inactivity do
                    where(ir.Beginning < DateTime.Now && ir.Ending > DateTime.Now && ir.Accepted = (sbyte 1))
                    count
                }) > 0 |> not
        }

    [<Rpc>]
    let requestInactivity sessionID start ending reason =
        async{
            try
            let db = Database.getDataContext()
            match User.getUserFromSID sessionID with
                |None -> return InvalidSession
                |Some u ->
                    let startDate = DateTime.Parse(start)
                    let endDate = DateTime.Parse(ending)
                    let overlap = 
                        (query{
                            for ir in db.Camblogistics.inactivity do
                            where(((ir.Beginning < startDate && ir.Ending > startDate) || (ir.Beginning > startDate && ir.Beginning < endDate)) && (ir.Pending = (sbyte 1) || ir.Accepted = (sbyte 1)))
                            count
                        }) > 0
                    if overlap then return Overlap 
                    else
                        let newRequest = db.Camblogistics.inactivity.Create()
                        newRequest.Accepted <- (sbyte 0)
                        newRequest.Pending <- (sbyte 1)
                        newRequest.Userid <- u.Id
                        newRequest.Beginning <- startDate
                        newRequest.Ending <- endDate
                        newRequest.Reason <- reason
                        db.SubmitUpdates()
                        return Success
            with
                _ -> return DatabaseError
        }
    [<Rpc>]
    let decideRequest sessionID req decision =
        async{
            try
            if not (Permission.checkPermission sessionID Permissions.InactivityAdmin) then ()
            else
                let db = Database.getDataContext()
                let request =
                    query{
                        for r in db.Camblogistics.inactivity do
                        where (r.Beginning = req.From && r.Ending = req.To && r.Userid = req.UserID && r.Pending = (sbyte 1))
                        exactlyOne
                    }
                request.Accepted <- if decision then sbyte 1 else sbyte 0
                request.Pending <- sbyte 0
                return db.SubmitUpdates()
                with
                    _ -> ()
            }
    [<Rpc>]
    let getPendingRequests sessionID =
        async{
            try
            if not (Permission.checkPermission sessionID Permissions.InactivityAdmin) then return []
            else
                let db = Database.getDataContext()
                return 
                    query{
                        for r in db.Camblogistics.inactivity do
                        join u in db.Camblogistics.users on (r.Userid = u.Id)
                        where(r.Pending = (sbyte 1))
                        select ({UserName = u.Name;UserID = u.Id;From = r.Beginning; To = r.Ending;Reason = r.Reason})
                    } |> Seq.toList
            with
                _ -> return []
        }

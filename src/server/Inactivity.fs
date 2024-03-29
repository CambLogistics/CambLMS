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
type UserInactivityStatus = {
        UserName: string
        UserID: int
        Status: bool
}

module Inactivity =
    let getActiveStatus (user:Member) =
            let db = Database.getDataContext()
            (query{
                for ir in db.Camblogistics.inactivity do
                    where(ir.Userid = user.Id && ir.Beginning < DateTime.Now && ir.Ending > DateTime.Now && ir.Accepted = (sbyte 1))
                    count
                }) > 0 |> not
    [<Rpc>]
    let getUserStatusList sessionID =
        async{
            try
            if not (Permission.checkPermission sessionID Permissions.InactivityAdmin) then return Error "Nem nézheted meg a felhasználói jogosultságokat!"
            else
            let! users = UserOperations.getUserList sessionID false false
            match users with
                |Ok ul ->
                    let (statusList,userList) = ul |> List.mapFold (fun l u -> (getActiveStatus u,u::l)) []
                    return Ok(List.rev userList |> List.zip statusList |> List.map (fun (s,u) -> {UserName = u.Name;UserID = u.Id; Status = s}))
                |Error e -> return Error e
            with
              e -> return Error e.Message
        }
    [<Rpc>]
    let requestInactivity (sessionID,start,ending,reason) =
        async{
            try
            let db = Database.getDataContext()
            match User.getUserFromSID sessionID with
                |None -> return ActionResult.InvalidSession
                |Some u ->
                    let startDate = DateTime.Parse(start)
                    let endDate = DateTime.Parse(ending)
                    let overlap = 
                        (query{
                            for ir in db.Camblogistics.inactivity do
                            where(((ir.Beginning < startDate && ir.Ending > startDate) || (ir.Beginning > startDate && ir.Beginning < endDate)) && ir.Userid = u.Id && (ir.Pending = (sbyte 1) || ir.Accepted = (sbyte 1)))
                            count
                        }) > 0
                    let existing = 
                        query{
                            for ir in db.Camblogistics.inactivity do
                            where(ir.Beginning = startDate && ir.Ending = endDate && u.Id = ir.Userid)
                            select ir
                        }
                    if overlap then return ActionResult.OtherError "Erre az időszakra(vagy egy részére) már van beadott kérelmed!"
                    else
                        let newRequest = if Seq.length existing > 0 then Seq.item 0 existing else db.Camblogistics.inactivity.Create()
                        newRequest.Accepted <- (sbyte 0)
                        newRequest.Pending <- (sbyte 1)
                        newRequest.Userid <- u.Id
                        newRequest.Beginning <- startDate
                        newRequest.Ending <- endDate
                        newRequest.Reason <- reason
                        db.SubmitUpdates()
                        return ActionResult.Success
            with
                _ -> return ActionResult.DatabaseError
        }
    [<Rpc>]
    let decideRequest (sessionID,(req:InactivityRequest),decision) =
        async{
            try
            if not (Permission.checkPermission sessionID Permissions.InactivityAdmin) then return InsufficientPermissions
            else
                let db = Database.getDataContext()
                let request =
                    query{
                        for r in db.Camblogistics.inactivity do
                        where (r.Userid = req.UserID && r.Pending = (sbyte 1))
                        select r
                    } |> Seq.filter 
                            (
                                fun r -> 
                                    r.Beginning.Subtract(req.From.ToLocalTime()).TotalMinutes < 1.0 && r.Ending.Subtract(req.To.ToLocalTime()).TotalMinutes < 1.0
                            ) 
                            |> Seq.item 0
                request.Accepted <- if decision then sbyte 1 else sbyte 0
                request.Pending <- sbyte 0
                db.SubmitUpdates()
                return ActionResult.Success
                with
                    e -> return OtherError e.Message
            }
    [<Rpc>]
    let getPendingRequests sessionID =
        async{
            try
            if not (Permission.checkPermission sessionID Permissions.InactivityAdmin) then return Error "Nincs jogosultságod a szabadságkérelmek kezeléséhez!"
            else
                let db = Database.getDataContext()
                return 
                    Ok(query{
                        for r in db.Camblogistics.inactivity do
                        join u in db.Camblogistics.users on (r.Userid = u.Id)
                        where(r.Pending = (sbyte 1))
                        select ({UserName = u.Name;UserID = u.Id;From = r.Beginning; To = r.Ending;Reason = r.Reason})
                    } |> Seq.toList)
            with
                e -> return Error e.Message
        }

namespace camblms

open WebSharper

[<JavaScript>]
type PendingFee = {ID: int;Username: string;Amount: int}

module ServiceFee =
    [<Rpc>]
    let getPendingFees sid =
        async{
        if not (Permission.checkPermission sid Permissions.ServiceFeeAdmin) then return Error "Nincs jogosultságod a szervizdíjak kezeléséhez!"
        else
        try
            let db = Database.getDataContext()
            return Ok (query{
                for fee in db.Camblogistics.servicefees do
                where (fee.Paid = (sbyte 0))
                join u in db.Camblogistics.users on (fee.UserId = u.Id)
                select({ID = fee.Id;Username = u.Name;Amount = fee.Amount})
            } |> Seq.toList)
        with
           e -> return Error e.Message
        }
    [<Rpc>]
    let submitPendingFee (sid, userid, amount) =
        async{
        if not (Permission.checkPermission sid Permissions.ServiceFeeAdmin) then return InsufficientPermissions
        else
        try
            let db = Database.getDataContext()
            let newFee = db.Camblogistics.servicefees.Create()
            newFee.Amount <- amount
            newFee.UserId <- userid
            newFee.Date <- System.DateTime.Now
            newFee.Paid <- (sbyte 0)
            db.SubmitUpdates()
            return ActionResult.Success
        with
            e -> return OtherError e.Message
        }
    [<Rpc>]
    let payFee (sid,feeID) =
        async{
        if not (Permission.checkPermission sid Permissions.ServiceFeeAdmin) then return InsufficientPermissions
        else
        try
        let db = Database.getDataContext()
        (query{
            for f in db.Camblogistics.servicefees do
            where(f.Id = feeID)
            exactlyOne
        }).Paid <- (sbyte 1)
        db.SubmitUpdates()
        return ActionResult.Success
        with
            e -> return OtherError e.Message
        }

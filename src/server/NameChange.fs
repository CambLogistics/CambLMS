namespace camblms
open WebSharper

[<JavaScript>]
type PendingChange = {UserID: int;OldName: string;NewName: string}

module NameChangeServer =
    [<Rpc>]
    let proposeNameChange (sid,oldname,newname,password) =
        async{
        let db = Database.getDataContext()
        let user = User.getUserFromSID sid
        let passwordOkay = User.authenticateLoggedInUser sid password
        match user with
            |None -> return ActionResult.InvalidSession
            |Some u -> 
                if u.Name = oldname |> not then return OtherError "Rosszul adtad meg a régi neved!"
                else 
                if not passwordOkay then return OtherError "Rossz jelszó!"
                else
                if String.length newname < 5 then return OtherError  "Az új név nem felel meg a követelményeknek!"
                else
                try
                    let existingChange =
                        query{
                            for x in db.Camblogistics.namechanges do
                            where(x.UserId = u.Id && x.Pending = (sbyte 1))
                            count
                        }
                    if existingChange > 0 then return OtherError "Már van elbírálásra váró kérelmed!"
                    else
                    let newChange = db.Camblogistics.namechanges.Create()
                    newChange.Approved <- (sbyte 0)
                    newChange.NewName <- newname
                    newChange.Pending <- sbyte 1
                    newChange.UserId <- u.Id
                    db.SubmitUpdates()
                    return ActionResult.Success
                with
                    _ -> return ActionResult.DatabaseError
        }
    [<Rpc>]
    let decideNameChange (sid,userID,decision) =
        async{
        if not (Permission.checkPermission sid Permissions.MemberAdmin) then return InsufficientPermissions
        else
        try
        let db = Database.getDataContext()
        let nc = 
            query{
                for x in db.Camblogistics.namechanges do
                where (x.UserId = userID && x.Pending = (sbyte 1))
                exactlyOne
            }
        nc.Pending <- (sbyte 0)
        if decision then
            nc.Approved <- (sbyte 1)
            let user = 
                query{
                    for u in db.Camblogistics.users do
                    where(u.Id = userID)
                    exactlyOne
                }
            user.Name <- nc.NewName
            db.SubmitUpdates()
            return ActionResult.Success
        else
            nc.Approved <- (sbyte 0)
            return ActionResult.Success
        with
            e -> return OtherError e.Message
        }
    [<Rpc>]
    let getPendingChanges sid = 
        async{
        if not (Permission.checkPermission sid Permissions.MemberAdmin) then return []
        else
        try
            let db = Database.getDataContext()
            return query{
                for x in db.Camblogistics.namechanges do
                join u in db.Camblogistics.users on (x.UserId = u.Id)
                where(x.Pending = (sbyte 1))
                select({UserID = x.UserId;OldName = u.Name;NewName = x.NewName})
            } |> Seq.toList
        with
            _ -> return []
        }
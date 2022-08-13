namespace camblms

open WebSharper

module UserOperations =
    [<Rpc>]
    let deleteUser (sid, userid) =
        async{
        let db = Database.getDataContext()
        if not (Permission.checkPermission sid Permissions.MemberAdmin) then return ActionResult.InsufficientPermissions
        else
        try
            let user =
                (query{
                    for user in db.Camblogistics.users do
                        where(user.Id = userid)
                        exactlyOne
                })
            user.Deleted <- (sbyte 1)
            query{
                for s in db.Camblogistics.sessions do
                    where(s.UserId = userid)
                    select s
            } |> Seq.iter(fun s -> s.Delete())
            db.SubmitUpdates()
            return ActionResult.Success
        with
            e -> return OtherError e.Message
        }
    [<Rpc>]
    let approveUser (sid, userid) =
        async{
        let db = Database.getDataContext()
        if not (Permission.checkPermission sid Permissions.MemberAdmin) then return InsufficientPermissions
        else
        try
            (query{
                for user in db.Camblogistics.users do
                    where(user.Id = userid)
                    exactlyOne
            }).Accepted <- (sbyte 1)
            db.SubmitUpdates()
            return ActionResult.Success
        with
            e -> return OtherError e.Message
        }
    [<Rpc>]
    let getUserList sid pending showDeleted =
        async{
        if not (Permission.checkPermission sid Permissions.MemberAdmin) then return []
        else
            let db = Database.getDataContext()
            return query{
                for user in db.Camblogistics.users do
                where(user.Accepted = (sbyte (if pending then 0 else 1)))
                select({Id = user.Id;Name = user.Name;Email = user.Email;AccountID = user.AccountId;Role = user.Role},user.Deleted)
            } |> Seq.toList |> List.filter(fun (u,d) -> if showDeleted then true else (d = sbyte 0)) |> List.map(fun (u,_) -> u) |> List.sortByDescending (fun u -> u.Role)
        }
    [<Rpc>]
    let changeUserRank (sid, userID, newRank) =
        async{ 
        if not (Permission.checkPermission sid Permissions.MemberAdmin) then return InsufficientPermissions
        else
        match User.getUserFromSID sid with
            |None -> return InvalidSession
            |Some u ->
                if u.Role <= (User.getUserByID userID).Value.Role || u.Id = userID then return InsufficientPermissions
                else
                try
                    let db = Database.getDataContext()
                    let user =
                        query{
                            for u in db.Camblogistics.users do
                            where(u.Id = userID)
                            exactlyOne
                        }
                    user.Role <- newRank
                    db.SubmitUpdates()
                    return ActionResult.Success
                with
                    e-> return OtherError e.Message
        }
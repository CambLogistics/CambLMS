namespace camblms

open WebSharper

module UserOperations =
    let deleteUser sid userid =
        let db = Database.getDataContext()
        if not (Permission.checkPermission sid Permissions.MemberAdmin) then ()
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
        with
            _ -> ()
    let approveUser sid userid =
        let db = Database.getDataContext()
        if not (Permission.checkPermission sid Permissions.MemberAdmin) then ()
        else
        try
            (query{
                for user in db.Camblogistics.users do
                    where(user.Id = userid)
                    exactlyOne
            }).Accepted <- (sbyte 1)
            db.SubmitUpdates()
        with
            _ -> ()
    let getUserList sid pending showDeleted =
        if not (Permission.checkPermission sid Permissions.MemberAdmin) then []
        else
            let db = Database.getDataContext()
            query{
                for user in db.Camblogistics.users do
                where(user.Accepted = (sbyte (if pending then 0 else 1)))
                select({Id = user.Id;Name = user.Name;Email = user.Email;AccountID = user.AccountId;Role = user.Role},user.Deleted)
            } |> Seq.toList |> List.filter(fun (u,d) -> if showDeleted then true else (d = sbyte 0)) |> List.map(fun (u,_) -> u) |> List.sortByDescending (fun u -> u.Role)
    let changeUserRank sid userID newRank = 
        if not (Permission.checkPermission sid Permissions.MemberAdmin) then ()
        else
        match User.getUserFromSID sid with
            |None -> ()
            |Some u ->
                if u.Role <= (User.getUserByID userID).Value.Role || u.Id = userID then ()
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
                with
                    _-> ()
    [<Rpc>]
    let doGetUserList sid pending showDeleted =
        async{
            return getUserList sid pending showDeleted
        }
    [<Rpc>]
    let doApproveUser sid userid =
        async{
            return approveUser sid userid
        }
    [<Rpc>]
    let doDeleteUser sid userid =
        async{
            return deleteUser sid userid
        }
    [<Rpc>]
    let doChangeUserPassword sid oldPassword newPassword =
        async{
            return User.changeUserPassword sid oldPassword newPassword 
        }
    [<Rpc>]
    let doChangeUserRank sid userID newRank =
        async{
            return changeUserRank sid userID newRank
        }
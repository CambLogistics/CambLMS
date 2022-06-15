namespace camblms

open WebSharper

module UserOperations =
    let deleteUser sid userid =
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        if not (User.verifyAdmin sid) then ()
        else
        try
            let user =
                (query{
                    for user in db.Camblogistics.users do
                        where(user.Id = userid)
                        exactlyOne
                })
            user.Deleted <- (sbyte 1)
            db.SubmitUpdates()
        with
            _ -> ()
    let approveUser sid userid =
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        if not (User.verifyAdmin sid) then ()
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
        if User.verifyAdmin sid |> not then []
        else
            let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
            query{
                for user in db.Camblogistics.users do
                where(user.Accepted = (sbyte (if pending then 0 else 1)))
                select({Id = user.Id;Name = user.Name;Email = user.Email;AccountID = user.AccountId;Role = user.Role},user.Deleted)
            } |> Seq.toList |> List.filter(fun (u,d) -> if showDeleted then true else (d = sbyte 0)) |> List.map(fun (u,_) -> u) |> List.sortByDescending (fun u -> u.Role)
    let changeUserRank sid userID newRank = 
        if not (User.verifyAdmin sid) then ()
        else
        match User.getUserFromSID sid with
            |None -> ()
            |Some u ->
                if u.Role <= (User.getUserByID userID).Value.Role || u.Id = userID then ()
                else
                try
                    let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
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
namespace camblms
open WebSharper

[<JavaScript>]
type NameChangeSuccess =
    |Success
    |WrongPassword
    |WrongOldName
    |WrongNewName
    |ChangeAlreadyPending
    |InvalidSession
    |DatabaseError

[<JavaScript>]
type PendingChange = {UserID: int;OldName: string;NewName: string}

module NameChangeServer =

    let proposeNameChange sid oldname newname password =
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        let user = User.getUserFromSID sid
        let passwordOkay = User.authenticateLoggedInUser sid password
        match user with
            |None -> InvalidSession
            |Some u -> 
                if u.Name = oldname |> not then WrongOldName
                else 
                if not passwordOkay then WrongPassword
                else
                if String.length newname < 5 then WrongNewName
                else
                try
                    let existingChange =
                        query{
                            for x in db.Camblogistics.namechanges do
                            where(x.UserId = u.Id && x.Pending = (sbyte 1))
                            count
                        }
                    if existingChange > 0 then ChangeAlreadyPending
                    else
                    let newChange = db.Camblogistics.namechanges.Create()
                    newChange.Approved <- (sbyte 0)
                    newChange.NewName <- newname
                    newChange.Pending <- sbyte 1
                    newChange.UserId <- u.Id
                    db.SubmitUpdates()
                    Success
                with
                    _ -> DatabaseError
    let decideNameChange sid userID decision =
        if User.verifyAdmin sid |> not then ()
        else
        try
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
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
        else
            nc.Approved <- (sbyte 0)
            db.SubmitUpdates()
        with
            _ -> ()
    let getPendingChanges sid = 
        if User.verifyAdmin sid |> not then []
        else
        try
            let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
            query{
                for x in db.Camblogistics.namechanges do
                join u in db.Camblogistics.users on (x.UserId = u.Id)
                where(x.Pending = (sbyte 1))
                select({UserID = x.UserId;OldName = u.Name;NewName = x.NewName})
            } |> Seq.toList
        with
            _ -> []

    [<Rpc>]
    let doProposeNameChange sid oldname newname password =
        async{
            return proposeNameChange sid oldname newname password
        }
    [<Rpc>]
    let doDecideNameChange sid userID decision =
        async{
            return decideNameChange sid userID decision
        }
    [<Rpc>]
    let doGetPendingChanges sid =
        async{
            return getPendingChanges sid
        }
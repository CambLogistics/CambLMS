namespace camblms

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open System.Security.Cryptography

[<JavaScript>]
type Member = {Id:int;Name: string;Role: int;AccountID: int;Email:string}
[<JavaScript>]
type Rank = {Level: int;Name:string}

type LoginStatus =
    |LoggedIn of Member
    |LoggedOut

[<JavaScript>]
type LoginResult =
    |Success of string
    |CredentialError
    |DatabaseError
    |NotApproved

[<JavaScript>]
type RegisterResult =
    |Success
    |Exists
    |MissingData
    |BadName
    |BadPassword
    |DatabaseError

[<JavaScript>]
type PasswordChangeResult =
    |Success
    |WrongPassword
    |BadNewPassword
    |InvalidSession
    |DatabaseError

module User =
    let minimumAdmin = 11
    let getUserFromSID sessionID = 
        try
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        let list =
            query{
                for session in db.Camblogistics.Sessions do
                    join user in db.Camblogistics.Users on (session.UserId = user.Id) 
                    where(session.Id = sessionID && session.Expiry > System.DateTime.Now && user.Deleted = (sbyte 0))
                    select({Id = user.Id;
                            Name = user.Name;
                            Role = user.Role; 
                            AccountID = user.AccountId;
                            Email = user.Email})
            } |> Seq.toList
        if (List.isEmpty list) then None
        else List.item 0 list |> Some
        with
            _ -> None
    let getUserByID userid =
        try
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        let list =
            query{
                for user in db.Camblogistics.Users do
                where(user.Id = userid)
                select({Id = user.Id;Name =user.Name;Role = user.Role;AccountID = user.AccountId;Email = user.Email})
            } |> Seq.toList
        if (List.isEmpty list) then None
        else List.item 0 list |> Some
        with
            _ -> None
    let verifyAdmin sessionID =
        match getUserFromSID sessionID with
            |None -> false
            |Some u -> u.Role >= minimumAdmin
    let hashPassword (password:string) = 
        let hash = SHA512.Create()
        (hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)) |> System.Convert.ToHexString).ToLower()
    let authenticateLoggedInUser sid password =
        try
        let dbContext = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        query{
            for user in dbContext.Camblogistics.Users do
            join session in dbContext.Camblogistics.Sessions on (user.Id = session.UserId)
            where (session.Id = sid && session.Expiry > System.DateTime.Now && user.Password = hashPassword password)
            select user
        } |> Seq.isEmpty |> not
        with
            _ -> false
    let generateSession userid =
        let dbContext = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        use rng = RandomNumberGenerator.Create()
        let mutable sessionRandom = Array.create 64 0uy
        rng.GetBytes sessionRandom
        let sessId = System.Convert.ToBase64String sessionRandom
        let expDate = System.DateTime.Now + System.TimeSpan(0,0,20,0)
        let newSession = dbContext.Camblogistics.Sessions.Create()
        newSession.UserId <- userid
        newSession.Id <- sessId
        newSession.Expiry <- expDate
        dbContext.SubmitUpdates()
        sessId
    let loginUser (name, password) =
        try
            let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
            let userList = query{
                    for user in db.Camblogistics.Users do
                    where(user.Name = name && user.Password = hashPassword password && user.Deleted = (sbyte 0))
                    select (user.Accepted,user.Id)
                    }
            if Seq.isEmpty userList then CredentialError
            else
                let (accepted,uid) = Seq.item 0 userList
                if accepted = (sbyte 0) then NotApproved
                else
                   (generateSession uid) |> LoginResult.Success
        with
            _ -> LoginResult.DatabaseError

    let registerUser (name,password,accountid,email) = 
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        if String.length password < 5 || String.length name < 3 || String.length email < 5 then MissingData
        else
            try
                let existing = 
                    query{
                        for user in db.Camblogistics.Users do
                        where(user.AccountId = accountid || user.Email = email)
                        select user
                    } |> Seq.toList
                if List.length existing > 0 then
                    (*If there is 2 or more accounts with the same accountID or Email, then there is huge trouble...
                    ..maybe handle it later*)
                    let user = List.item 0 existing
                    if user.Deleted = (sbyte 0) then Exists
                    else
                        user.Deleted <- (sbyte 0)
                        user.Accepted <- (sbyte 0)
                        db.SubmitUpdates()
                        RegisterResult.Success
                else
                    let newUser = db.Camblogistics.Users.Create()
                    newUser.Accepted <- sbyte 0
                    newUser.AccountId <- accountid
                    newUser.Role <- 0
                    newUser.Email <- email
                    newUser.Name <- name
                    newUser.Password <- hashPassword password
                    newUser.Deleted <- sbyte 0
                    db.SubmitUpdates()
                    RegisterResult.Success
            with
                _ -> RegisterResult.DatabaseError
    let logoutUser (ctx:Context<EndPoint>) =
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        (query{
            for session in db.Camblogistics.Sessions do
                where(session.Id = (ctx.Request.Cookies.Item "clms_sid").Value)
                select session
            }) |> Seq.iter(fun s -> s.Delete())
        db.SubmitUpdates()
    let deleteUser sid userid =
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        if not (verifyAdmin sid) then ()
        else
        try
            let user =
                (query{
                    for user in db.Camblogistics.Users do
                        where(user.Id = userid)
                        exactlyOne
                })
            user.Deleted <- (sbyte 1)
            db.SubmitUpdates()
        with
            _ -> ()
    let approveUser sid userid =
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        if not (verifyAdmin sid) then ()
        else
        try
            (query{
                for user in db.Camblogistics.Users do
                    where(user.Id = userid)
                    exactlyOne
            }).Accepted <- (sbyte 1)
            db.SubmitUpdates()
        with
            _ -> ()
    let getUserList sid pending =
        if verifyAdmin sid |> not then []
        else
            let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
            query{
                for user in db.Camblogistics.Users do
                where (user.Deleted = (sbyte 0) && user.Accepted = (sbyte (if pending then 0 else 1)))
                select({Id = user.Id;Name = user.Name;Email = user.Email;AccountID = user.AccountId;Role = user.Role})
            } |> Seq.toList |> List.sortBy (fun u -> u.Id)
    let getRankList() =
        try
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        query{
            for r in db.Camblogistics.Roles do
            select({Level = r.Id;Name = r.Name})
        } |> Seq.toList |> List.sortBy(fun r -> r.Level)
        with
            _ -> []
    let changeUserRank sid userID newRank = 
        if not (verifyAdmin sid) then ()
        else
        match getUserFromSID sid with
            |None -> ()
            |Some u ->
                if u.Role <= (getUserByID userID).Value.Role || u.Id = userID then ()
                else
                try
                    let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
                    let user =
                        query{
                            for u in db.Camblogistics.Users do
                            where(u.Id = userID)
                            exactlyOne
                        }
                    user.Role <- newRank
                    db.SubmitUpdates()
                with
                    _-> ()
    let changeUserPassword sid oldPassword newPassword =
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        if String.length newPassword < 3 then BadNewPassword
        else
        if authenticateLoggedInUser sid oldPassword |> not then PasswordChangeResult.WrongPassword
        else
        match getUserFromSID sid with
            |None -> PasswordChangeResult.InvalidSession
            |Some user ->
                try
                    let userEntry =
                        query{
                            for u in db.Camblogistics.Users do
                            where(u.Id = user.Id)
                            exactlyOne
                        }
                    userEntry.Password <- hashPassword newPassword
                    db.SubmitUpdates()
                    Success
                with
                    _ -> PasswordChangeResult.DatabaseError
    let lengthenSession sid =
        try
        let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
        (query{
            for s in db.Camblogistics.Sessions do
            where (s.Id = sid)
            exactlyOne
        }).Expiry <- System.DateTime.Now + System.TimeSpan(0,0,20,0)
        db.SubmitUpdates()
        with
            _ -> ()

module UserCallable =
    [<Rpc>]
    let doLogin username password =
        async{
            return User.loginUser(username,password)
        }
    [<Rpc>]
    let doRegister name password accountId email =
        async{
            return User.registerUser(name,password,accountId,email)
        }
    [<Rpc>]
    let getUser sid =
        async{
            return User.getUserFromSID sid
        }
    [<Rpc>]
    let doGetUserList sid pending =
        async{
            return User.getUserList sid pending
        }
    [<Rpc>]
    let doApproveUser sid userid =
        async{
            return User.approveUser sid userid
        }
    [<Rpc>]
    let doDeleteUser sid userid =
        async{
            return User.deleteUser sid userid
        }
    [<Rpc>]
    let doChangeUserPassword sid oldPassword newPassword =
        async{
            return User.changeUserPassword sid oldPassword newPassword 
        }
    [<Rpc>]
    let doChangeUserRank sid userID newRank =
        async{
            return User.changeUserRank sid userID newRank
        }
    [<Rpc>]
    let doGetRankList() =
        async{
            return User.getRankList()
        }
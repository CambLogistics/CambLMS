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
    let getUserFromSID sessionID = 
        try
        let db = Database.getDataContext()
        let list =
            query{
                for session in db.Camblogistics.sessions do
                    join user in db.Camblogistics.users on (session.UserId = user.Id) 
                    where(session.Id = sessionID && session.Expiry > System.DateTime.Now && user.Deleted = (sbyte 0) && user.Accepted = (sbyte 1))
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
        let db = Database.getDataContext()
        let list =
            query{
                for user in db.Camblogistics.users do
                where(user.Id = userid)
                select({Id = user.Id;Name =user.Name;Role = user.Role;AccountID = user.AccountId;Email = user.Email})
            } |> Seq.toList
        if (List.isEmpty list) then None
        else List.item 0 list |> Some
        with
            _ -> None
    let hashPassword (password:string) = 
        let hash = SHA512.Create()
        (hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)) |> System.Convert.ToHexString).ToLower()
    let authenticateLoggedInUser sid password =
        try
        let dbContext = Database.getDataContext()
        query{
            for user in dbContext.Camblogistics.users do
            join session in dbContext.Camblogistics.sessions on (user.Id = session.UserId)
            where (session.Id = sid && session.Expiry > System.DateTime.Now && user.Password = hashPassword password)
            select user
        } |> Seq.isEmpty |> not
        with
            _ -> false
    let generateSession userid =
        let dbContext = Database.getDataContext()
        use rng = RandomNumberGenerator.Create()
        let mutable sessionRandom = Array.create 64 0uy
        rng.GetBytes sessionRandom
        let sessId = System.Convert.ToBase64String sessionRandom
        let expDate = System.DateTime.Now + System.TimeSpan(0,0,20,0)
        let newSession = dbContext.Camblogistics.sessions.Create()
        newSession.UserId <- userid
        newSession.Id <- sessId
        newSession.Expiry <- expDate
        dbContext.SubmitUpdates()
        sessId
    let loginUser (name, password) =
        try
            let db = Database.getDataContext()
            let userList = query{
                    for user in db.Camblogistics.users do
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
        let db = Database.getDataContext()
        if String.length password < 5 || String.length name < 3 || String.length email < 5 then MissingData
        else
            try
                let existing = 
                    query{
                        for user in db.Camblogistics.users do
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
                    let newUser = db.Camblogistics.users.Create()
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
        let db = Database.getDataContext()
        (query{
            for session in db.Camblogistics.sessions do
                where(session.Id = (ctx.Request.Cookies.Item "clms_sid").Value)
                select session
            }) |> Seq.iter(fun s -> s.Delete())
        db.SubmitUpdates()
    
    let getRankList() =
        try
        let db = Database.getDataContext()
        query{
            for r in db.Camblogistics.roles do
            select({Level = r.Id;Name = r.Name})
        } |> Seq.toList |> List.sortBy(fun r -> r.Level)
        with
            _ -> []
    let changeUserPassword sid oldPassword newPassword =
        let db = Database.getDataContext()
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
                            for u in db.Camblogistics.users do
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
        let db = Database.getDataContext()
        (query{
            for s in db.Camblogistics.sessions do
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
    let doGetRankList() =
        async{
            return User.getRankList()
        }
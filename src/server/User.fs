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
        let dbContext = Database.getDataContext()
        query{
            for user in dbContext.Camblogistics.users do
            join session in dbContext.Camblogistics.sessions on (user.Id = session.UserId)
            where (session.Id = sid && session.Expiry > System.DateTime.Now && user.Password = hashPassword password)
            select user
        } |> Seq.isEmpty |> not
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
    let getFirstName (user:Member) =
        let name = System.String(user.Name)
        (((name.Split ' ')[0]).Split '_')[0]
    [<Rpc>]
    let loginUser (name, password) =
        async{
        try
            let db = Database.getDataContext()
            let userList = query{
                    for user in db.Camblogistics.users do
                    where(user.Name = name && user.Password = hashPassword password && user.Deleted = (sbyte 0))
                    select (user.Accepted,user.Id)
                    }
            if Seq.isEmpty userList then return CredentialError
            else
                let (accepted,uid) = Seq.item 0 userList
                if accepted = (sbyte 0) then return NotApproved
                else
                   return (generateSession uid) |> LoginResult.Success
        with
            _ -> return LoginResult.DatabaseError
        }
    [<Rpc>]
    let registerUser (name,password,accountid,email) =
        async{ 
        let db = Database.getDataContext()
        if String.length password < 5 || String.length name < 3 || String.length email < 5 then return MissingData
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
                    if user.Deleted = (sbyte 0) then return Exists
                    else
                        user.Deleted <- (sbyte 0)
                        user.Accepted <- (sbyte 0)
                        db.SubmitUpdates()
                        return RegisterResult.Success
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
                    return RegisterResult.Success
            with
                _ -> return RegisterResult.DatabaseError
        }
    let logoutUser (ctx:Context<EndPoint>) =
        let db = Database.getDataContext()
        (query{
            for session in db.Camblogistics.sessions do
                where(session.Id = (ctx.Request.Cookies.Item "clms_sid").Value)
                select session
            }) |> Seq.iter(fun s -> s.Delete())
        db.SubmitUpdates()
    
    [<Rpc>]
    let getRankList() =
        async{
        try
            let db = Database.getDataContext()
            return Ok(query{
                for r in db.Camblogistics.roles do
                select({Level = r.Id;Name = r.Name})
            } |> Seq.toList |> List.sortBy(fun r -> r.Level)
            )
        with
            e -> return Error e.Message
        }
    [<Rpc>]
    let changeUserPassword (sid, oldPassword, newPassword) =
        async{
        try
        let db = Database.getDataContext()
        if String.length newPassword < 3 then return ActionResult.OtherError "Ez a jelszó nem megfelelő!"
        else
        if authenticateLoggedInUser sid oldPassword |> not then return ActionResult.OtherError "A régi jelszót rosszul adtad meg!"
        else
        match getUserFromSID sid with
            |None -> return ActionResult.InvalidSession
            |Some user ->
                let userEntry =
                    query{
                        for u in db.Camblogistics.users do
                        where(u.Id = user.Id)
                        exactlyOne
                    }
                userEntry.Password <- hashPassword newPassword
                db.SubmitUpdates()
                return ActionResult.Success
        with
            _ -> return ActionResult.DatabaseError
        }
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
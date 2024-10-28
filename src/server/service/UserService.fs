namespace camblms.server.service

open FSharp.Data.Sql
open System.Security.Cryptography

open camblms.dto
open camblms.server.database

module UserService =
    let hashPassword (password: string) =
        let hash = SHA512.Create()

        (hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))
         |> System.Convert.ToHexString)
            .ToLower()

    let generateSession userid =
        let dbContext = Database.getDataContext ()
        use rng = RandomNumberGenerator.Create()
        let mutable sessionRandom = Array.create 64 0uy
        rng.GetBytes sessionRandom
        let sessId = System.Convert.ToBase64String sessionRandom
        let expDate = System.DateTime.Now + System.TimeSpan(0, 0, 20, 0)
        let newSession = dbContext.Camblogistics.sessions.Create()
        newSession.UserId <- userid
        newSession.Id <- sessId
        newSession.Expiry <- expDate
        dbContext.SubmitUpdates()
        sessId

    let getFirstName (user: Member) =
        let name = System.String(user.Name)
        (((name.Split ' ')[0]).Split '_')[0]

    let getUserFromSID sessionID =
        try
            let db = Database.getDataContext ()

            let list =
                query {
                    for session in db.Camblogistics.sessions do
                        join user in db.Camblogistics.users on (session.UserId = user.Id)
                        join role in db.Camblogistics.roles on (user.Role = role.Id)

                        where (
                            session.Id = sessionID
                            && session.Expiry > System.DateTime.Now
                            && user.Deleted = (sbyte 0)
                            && user.Accepted = (sbyte 1)
                        )

                        select (
                            { Id = user.Id
                              Name = user.Name
                              Role = { Level = user.Role; Name = role.Name }
                              AccountID = user.AccountId
                              Email = user.Email }
                        )
                }

            if (Seq.isEmpty list) then None else Seq.item 0 list |> Some
        with e ->
            None

    let getUserByID userid =
        try
            let db = Database.getDataContext ()

            let list =
                query {
                    for user in db.Camblogistics.users do
                        join role in db.Camblogistics.roles on (user.Role = role.Id)
                        where (user.Id = userid)

                        select (
                            { Id = user.Id
                              Name = user.Name
                              Role = { Level = user.Role; Name = role.Name }
                              AccountID = user.AccountId
                              Email = user.Email }
                        )
                }
                |> Seq.toList

            if (List.isEmpty list) then
                None
            else
                List.item 0 list |> Some
        with _ ->
            None

    let getAllUsers () =
        let db = Database.getDataContext ()

        query {
            for user in db.Camblogistics.users do
                join role in db.Camblogistics.roles on (user.Role = role.Id)

                select (
                    { Id = user.Id
                      Name = user.Name
                      Role = { Level = user.Role; Name = role.Name }
                      AccountID = user.AccountId
                      Email = user.Email }
                )
        }

    let loginUser name password =
        let db = Database.getDataContext ()

        let userList =
            query {
                for user in db.Camblogistics.users do
                    where (
                        user.Name = name
                        && user.Password = hashPassword password
                        && user.Deleted = (sbyte 0)
                    )

                    select (user.Accepted, user.Id)
            }

        if Seq.isEmpty userList then
            LoginResult.CredentialError
        else
            let (accepted, uid) = Seq.item 0 userList

            if accepted = (sbyte 0) then
                LoginResult.NotApproved
            else
                (generateSession uid) |> LoginResult.Success

    let registerUser (name, password, accountid, email) =
        let db = Database.getDataContext ()

        let existing =
            query {
                for user in db.Camblogistics.users do
                    where (user.AccountId = accountid || user.Email = email)
                    select user
            }

        if Seq.length existing > 0 then
            let user = Seq.item 0 existing

            if user.Deleted = (sbyte 0) then
                Exists
            else
                user.Deleted <- (sbyte 0)
                user.Accepted <- (sbyte 0)
                do db.SubmitUpdates()
                RegisterResult.Success
        else
            let newUser = db.Camblogistics.users.Create()
            newUser.Accepted <- sbyte 0
            newUser.AccountId <- accountid
            newUser.Role <- 1
            newUser.Email <- email
            newUser.Name <- name
            newUser.Password <- hashPassword password
            newUser.Deleted <- sbyte 0
            do db.SubmitUpdates()
            RegisterResult.Success

    let deleteUser userid =
        let db = Database.getDataContext ()

        let userEntity =
            query {
                for u in db.Camblogistics.users do
                    where (u.Id = userid)
                    exactlyOne
            }

        userEntity.Deleted <- (sbyte 1)

        if System.IO.File.Exists(@"docs/" + string userEntity.AccountId + "_license.png") then
            System.IO.File.Delete(@"docs/" + string userEntity.AccountId + "_license.png")

        if System.IO.File.Exists(@"docs/" + string userEntity.AccountId + "_personal.png") then
            System.IO.File.Delete(@"docs/" + string userEntity.AccountId + "_personal.png")

        let sessions =
            query {
                for s in db.Camblogistics.sessions do
                    where (s.UserId = userid)
                    select s
            }

        sessions |> Seq.iter (fun s -> s.Delete())

        let cars =
            query {
                for c in db.Camblogistics.cars do
                    where (c.KeyHolder1 = Some userEntity.Id)
                    select c
            }

        cars |> Seq.iter (fun c -> (c.KeyHolder1 <- None))
        db.SubmitUpdates()

    let approveUser userid =
        let db = Database.getDataContext ()

        let user =
            query {
                for u in db.Camblogistics.users do
                    where (u.Id = userid)
                    exactlyOne
            }

        user.Accepted <- sbyte 1
        do db.SubmitUpdates()

    let getRankList () =
        let db = Database.getDataContext ()

        query {
            for r in db.Camblogistics.roles do
                where (not (r.Id = 0))
                sortBy r.Id
                select ({ Level = r.Id; Name = r.Name })
        }
        |> Seq.toList

    let getUserList pending showDeleted =
        let db = Database.getDataContext ()

        query {
            for user in db.Camblogistics.users do
                join role in db.Camblogistics.roles on (user.Role = role.Id)

                where (
                    user.Accepted = (sbyte (if pending then 0 else 1))
                    && user.Deleted = (sbyte (if showDeleted then 0 else 1))
                )

                sortByDescending user.Role

                select (
                    { Id = user.Id
                      Name = user.Name
                      Email = user.Email
                      AccountID = user.AccountId
                      Role = { Level = user.Role; Name = role.Name } }
                )
        }
        |> Seq.toList

    let authenticateLoggedInUser sid password =
        let dbContext = Database.getDataContext ()

        query {
            for user in dbContext.Camblogistics.users do
                join session in dbContext.Camblogistics.sessions on (user.Id = session.UserId)

                where (
                    session.Id = sid
                    && session.Expiry > System.DateTime.Now
                    && user.Password = hashPassword password
                )

                select user
        }
        |> Seq.isEmpty
        |> not

    let lengthenSession sid =
        try
            let db = Database.getDataContext ()

            (query {
                for s in db.Camblogistics.sessions do
                    where (s.Id = sid)
                    exactlyOne
            })
                .Expiry <- System.DateTime.Now + System.TimeSpan(0, 0, 20, 0)

            db.SubmitUpdates()
        with _ ->
            ()

    let changeUserPassword (user: Member) newPassword =
        let db = Database.getDataContext ()

        let userEntry =
            query {
                for u in db.Camblogistics.users do
                    where (u.Id = user.Id)
                    exactlyOne
            }

        userEntry.Password <- hashPassword newPassword
        db.SubmitUpdates()

    let changeUserRank userId newRank =
        let db = Database.getDataContext ()

        let user =
            query {
                for u in db.Camblogistics.users do
                    where (u.Id = userId)
                    exactlyOne
            }

        user.Role <- newRank
        db.SubmitUpdates()

    let getUserByEmail email =
        let db = Database.getDataContext ()

        let userSeq =
            query {
                for u in db.Camblogistics.users do
                    join role in db.Camblogistics.roles on (u.Role = role.Id)
                    where (u.Email = email && u.Accepted = (sbyte 1) && u.Deleted = (sbyte 0))

                    select
                        { Id = u.Id
                          Name = u.Name
                          AccountID = u.AccountId
                          Role = { Level = u.Role; Name = role.Name }
                          Email = u.Email }
            }

        if Seq.length userSeq > 0 then
            Some <| Seq.item 0 userSeq
        else
            None

    let logoutUser sid =
        let db = Database.getDataContext ()

        (query {
            for session in db.Camblogistics.sessions do
                where (session.Id = sid || session.Expiry < System.DateTime.Now)
                select session
        })
        |> Seq.iter (fun s -> s.Delete())

        db.SubmitUpdates()

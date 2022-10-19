namespace camblms

open WebSharper

[<JavaScript>]
type BlacklistItem =
    {
        UserName: string
        Role: int
        AccountID: int
        Reason: string
        Comeback: bool
    }

module Blacklist =
    [<Rpc>]
    let getBlackListItems sid =
        async{
        try
        if not (Permission.checkPermission sid Permissions.MemberAdmin) then return Error "Nincs jogosultságod a felhasználók kezeléséhez!"
        else
            let db = Database.getDataContext()
            return Ok (query {
                for i in db.Camblogistics.blacklist do
                    select{
                        UserName = i.Name
                        Role = i.RoleId
                        AccountID = i.AccountId
                        Reason = i.Reason
                        Comeback = (i.CanReturn = sbyte 1)
                    }
            } |> Seq.toList)
        with e -> return Error e.Message
        }
    [<Rpc>]
    let setBlackListItem (sid,bli) =
        async{
        try
            if not (Permission.checkPermission sid Permissions.MemberAdmin) then return ActionResult.InsufficientPermissions
            else
                let db = Database.getDataContext()
                let existing = query{
                    for i in db.Camblogistics.blacklist do
                        where(i.AccountId = bli.AccountID)
                        select i
                }
                let newItem = if Seq.length existing > 0 then Seq.item 0 existing else db.Camblogistics.blacklist.Create()
                newItem.AccountId <- bli.AccountID
                newItem.CanReturn <- if bli.Comeback then sbyte 1 else sbyte 0
                newItem.Reason <- bli.Reason
                newItem.Name <- bli.UserName
                newItem.RoleId <- bli.Role
                db.SubmitUpdates()
                return ActionResult.Success

        with e -> return ActionResult.DatabaseError
        }
    [<Rpc>]
    let deleteItem (sid,bli) =
        async{
            try
                if not (Permission.checkPermission sid Permissions.MemberAdmin) then return ActionResult.InsufficientPermissions
                else
                    let db = Database.getDataContext()
                    let existing = query {
                        for i in db.Camblogistics.blacklist do
                            where(i.AccountId = bli.AccountID)
                            select i
                    }
                    if Seq.length existing = 0 then return ActionResult.OtherError "Nem lehet nem létező feketelista elemet törölni!"
                    else
                        (Seq.item 0 existing).Delete()
                        db.SubmitUpdates()
                        return ActionResult.Success
            with e -> return ActionResult.DatabaseError
        }
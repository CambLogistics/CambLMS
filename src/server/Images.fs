namespace camblms

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI

module Documents =
    
    [<Rpc>]
    let getUsersWithValidDocuments sid =
      async{
        try
            if not (Permission.checkPermission sid Permissions.DocAdmin) then return Error "Nem vagy jogosult lekérni az iratokat!"
            else
            let db = Database.getDataContext()
            return Ok(query{
                for u in db.Camblogistics.users do
                where(u.Accepted = (sbyte 1))
                select({Id = u.Id;
                            Name = u.Name;
                            Role = -1; 
                            AccountID = u.AccountId;
                            Email = ""})
            } |> Seq.toList |> List.filter (
                fun u ->
                    System.Diagnostics.Debug.WriteLine <| string (System.IO.File.Exists(@"docs/" + string u.AccountID + "_personal.png"))
                    System.IO.File.Exists(@"docs/" + string u.AccountID + "_personal.png") &&
                    System.IO.File.Exists(@"docs/" + string u.AccountID + "_license.png")
            ))
        with
            e  -> return Error e.Message
      }
module ImageUpload =
    
    [<Rpc>]
    let DeleteImage(sid,filename) =
        async{
        try
            if not (Permission.checkPermission sid Permissions.ServiceFeeAdmin) then return InsufficientPermissions
            else
                let db = Database.SqlConnection.GetDataContext(Database.getConnectionString())
                if System.IO.File.Exists(@"img/" + filename) then System.IO.File.Delete(@"img/" + filename)
                (query{
                    for f in db.Camblogistics.images do
                    where(f.Name = filename)
                    exactlyOne
                }).Delete()
                db.SubmitUpdates()
                return ActionResult.Success
        with
           | e -> return OtherError e.Message
        }
    [<Rpc>]
    let getImageList sid =
      async{
        try
            if not (Permission.checkPermission sid Permissions.ServiceFeeAdmin) then return Error "Nincs jogosultságod a szervízképekhez!"
            else
            let db = Database.SqlConnection.GetDataContext(Database.getConnectionString())
            return Ok(query{
                for i in db.Camblogistics.images do
                select (i.Name,i.Userid,i.UploadDate)
            } |> Seq.toList |> List.sortByDescending (fun (_,_,date) -> date))
        with
         e -> return Error e.Message
      }

module ImageSubmitter =
    let Documents (ctx:Context<EndPoint>) (user:Member) =
        try
            let (personal,license) =
                (query{
                    for file in ctx.Request.Files do
                        where(file.Key = "personal")
                        exactlyOne
                },
                query{
                   for file in ctx.Request.Files do
                        where(file.Key = "license")
                        exactlyOne 
                })
            if System.IO.Directory.Exists "docs" |> not then System.IO.Directory.CreateDirectory "docs" |> ignore
            personal.SaveAs (@"docs/" + string user.AccountID + "_personal" + System.IO.Path.GetExtension personal.FileName)
            license.SaveAs (@"docs/" + string user.AccountID + "_license" + System.IO.Path.GetExtension license.FileName)
            Content.RedirectTemporaryToUrl((ctx.Link EndPoint.Documents) + "?success=true")
            with
                _ -> Content.RedirectTemporaryToUrl((ctx.Link EndPoint.Documents) + "?success=false")

    let Images (ctx:Context<EndPoint>) (user:Member) =
        try
            let file = Seq.item 0 ctx.Request.Files
            let filename = RandomString.getRandomString 32 + System.IO.Path.GetExtension file.FileName
            if not (Inactivity.getActiveStatus user) then Content.RedirectTemporaryToUrl((ctx.Link EndPoint.ImageUpload) + "?success=inactivity")
            else
            let db = Database.getDataContext()
            if System.IO.Directory.Exists "img" |> not then System.IO.Directory.CreateDirectory "img" |> ignore
            file.SaveAs (@"img/" + filename)
            let newFileEntry = db.Camblogistics.images.Create()
            newFileEntry.Userid <- user.Id
            newFileEntry.Name <- filename
            newFileEntry.UploadDate <- System.DateTime.Now
            db.SubmitUpdates()
            Content.RedirectTemporaryToUrl((ctx.Link EndPoint.ImageUpload) + "?success=true")
        with
            _ -> Content.RedirectTemporaryToUrl((ctx.Link EndPoint.ImageUpload) + "?success=error")
module ImageServe =
    let Docs (ctx:Context<EndPoint>) fn =
        match ctx.Request.Cookies.Item "clms_sid" with
            |None -> Content.RedirectTemporary(EndPoint.Home)
            |Some s -> 
                if Permission.checkPermission s Permissions.DocAdmin then
                    if System.IO.File.Exists("docs/" + fn) then Content.File("../docs/" + fn,true)
                    else Content.NotFound
                else Content.Forbidden
    let Service (ctx:Context<EndPoint>) fn =
        match ctx.Request.Cookies.Item "clms_sid" with
            |None -> Content.RedirectTemporary(EndPoint.Home)
            |Some s -> 
                if Permission.checkPermission s Permissions.ServiceFeeAdmin then
                    if System.IO.File.Exists("img/" + fn) then Content.File("../img/" + fn,true)
                    else Content.NotFound
                else Content.Forbidden

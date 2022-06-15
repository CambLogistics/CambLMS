namespace camblms

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI

module Documents =
    let MakePage ctx =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx false)
            .Stylesheet(SiteTemplates.NormalStyle)
            .Main(
                SiteParts.DocumentsTemplate()
                    .Message(
                        match ctx.Request.Get.Item "success" with
                            |Some s -> 
                                if s = "true" then 
                                    SiteParts.DocumentsTemplate.Success().Message("Sikeres dokumentumbeküldés!").Doc()
                                else 
                                    SiteParts.DocumentsTemplate.Error().Message("Hiba feltöltés közben! Keresd a (műszaki) igazgatót!").Doc()
                            |None -> Doc.Empty
                    )
                    .Doc()
            )
            .Doc()
    let getUsersWithValidDocuments sid =
        try
            if not (Permission.checkPermission sid Permissions.DocAdmin) then []
            else
            let db = Database.SqlConnection.GetDataContext(Database.getConnectionString())
            query{
                for u in db.Camblogistics.users do
                where(u.Accepted = (sbyte 1))
                select({Id = u.Id;
                            Name = u.Name;
                            Role = -1; 
                            AccountID = u.AccountId;
                            Email = ""})
            } |> Seq.toList |> List.filter (
                fun u -> 
                    System.IO.File.Exists(@"wwwroot/docs/" + string u.AccountID + "_personal.png") &&
                    System.IO.File.Exists(@"wwwroot/docs/" + string u.AccountID + "_license.png")
            )
        with
            _ -> []
    [<Rpc>]
    let doGetUsersWithDocuments sid =
        async{
            return getUsersWithValidDocuments sid
        } 
module ImageUpload =
    let MakePage ctx =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx false)
            .Stylesheet(SiteTemplates.NormalStyle)
            .Main(
                SiteParts.ImageUploadTemplate()
                    .Message(
                        match ctx.Request.Get.Item "success" with
                            |Some s -> 
                                if s = "true" then 
                                    SiteParts.DocumentsTemplate.Success().Message("Sikeres képfeltöltés!").Doc()
                                else 
                                    SiteParts.DocumentsTemplate.Error().Message("Hiba feltöltés közben! Keresd a (műszaki) igazgatót!").Doc()
                            |None -> Doc.Empty
                    )
                    .Doc()
            )
            .Doc()
    let DeleteImage sid filename =
        try
            if not (Permission.checkPermission sid Permissions.ServiceFeeAdmin) then ()
            else
                let db = Database.SqlConnection.GetDataContext(Database.getConnectionString())
                if System.IO.File.Exists(@"wwwroot/img/" + filename) then System.IO.File.Delete(@"wwwroot/img/" + filename)
                (query{
                    for f in db.Camblogistics.images do
                    where(f.Name = filename)
                    exactlyOne
                }).Delete()
                db.SubmitUpdates()
        with
            _ -> ()
    let getImageList sid =
        try
            if not (Permission.checkPermission sid Permissions.ServiceFeeAdmin)then []
            else
            let db = Database.SqlConnection.GetDataContext(Database.getConnectionString())
            query{
                for i in db.Camblogistics.images do
                select (i.Name,i.Userid,i.UploadDate)
            } |> Seq.toList |> List.sortByDescending (fun (_,_,date) -> date)
        with
         _ -> []
    [<Rpc>]
    let doGetImageList sid =
        async{
            return getImageList sid
        }
    [<Rpc>]
    let doDeleteImage sid fn =
        async{
            return DeleteImage sid fn
        }

module ImageSubmitter =
    let getRandomString length =
        let chars = "ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvxyz0123456789"
        let rnd = System.Random()
        System.String(Array.init length (fun _ -> chars[rnd.Next chars.Length]))
    let Documents (ctx:Context<EndPoint>) user =
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
            if System.IO.Directory.Exists "wwwroot/docs" |> not then System.IO.Directory.CreateDirectory "wwwroot/docs" |> ignore
            personal.SaveAs (@"wwwroot/docs/" + string user.AccountID + "_personal" + System.IO.Path.GetExtension personal.FileName)
            license.SaveAs (@"wwwroot/docs/" + string user.AccountID + "_license" + System.IO.Path.GetExtension license.FileName)
            Content.RedirectTemporaryToUrl((ctx.Link EndPoint.Documents) + "?success=true")
            with
                _ -> Content.RedirectTemporaryToUrl((ctx.Link EndPoint.Documents) + "?success=false")

    let Images (ctx:Context<EndPoint>) (user:Member) =
        try
            let file = Seq.item 0 ctx.Request.Files
            let filename = getRandomString 32 + System.IO.Path.GetExtension file.FileName
            let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
            if System.IO.Directory.Exists "wwwroot/img" |> not then System.IO.Directory.CreateDirectory "wwwroot/img" |> ignore
            file.SaveAs (@"wwwroot/img/" + filename)
            let newFileEntry = db.Camblogistics.images.Create()
            newFileEntry.Userid <- user.Id
            newFileEntry.Name <- filename
            newFileEntry.UploadDate <- System.DateTime.Now
            db.SubmitUpdates()
            Content.RedirectTemporaryToUrl((ctx.Link EndPoint.ImageUpload) + "?success=true")
        with
            _ -> Content.RedirectTemporaryToUrl((ctx.Link EndPoint.ImageUpload) + "?success=false")
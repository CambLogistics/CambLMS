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
        //try
            if not (Permission.checkPermission sid Permissions.DocAdmin) then []
            else
            let db = Database.getDataContext()
            
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
                    System.Diagnostics.Debug.WriteLine <| string (System.IO.File.Exists(@"docs/" + string u.AccountID + "_personal.png"))
                    System.IO.File.Exists(@"docs/" + string u.AccountID + "_personal.png") &&
                    System.IO.File.Exists(@"docs/" + string u.AccountID + "_license.png")
            )
        //with
        //    _ -> []
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
                                else if s = "inactivity" then
                                    SiteParts.DocumentsTemplate.Success().Message("Szabadság alatt nem tölthetsz fel képet!").Doc()
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
                if System.IO.File.Exists(@"img/" + filename) then System.IO.File.Delete(@"img/" + filename)
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

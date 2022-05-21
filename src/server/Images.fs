namespace camblms

open WebSharper.Sitelets
open WebSharper.UI

module Documents =
    let MakePage ctx =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx (EndPoint.Documents))
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
module ImageUpload =
    let MakePage ctx =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx (EndPoint.ImageUpload))
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
            let db = Database.SqlConnection.GetDataContext()
            if System.IO.Directory.Exists "wwwroot/img" |> not then System.IO.Directory.CreateDirectory "wwwroot/img" |> ignore
            file.SaveAs (@"wwwroot/img/" + filename)
            let newFileEntry = db.Camblogistics.Images.Create()
            newFileEntry.Userid <- user.Id
            newFileEntry.Name <- filename
            db.SubmitUpdates()
            Content.RedirectTemporaryToUrl((ctx.Link EndPoint.ImageUpload) + "?success=true")
        with
            _ -> Content.RedirectTemporaryToUrl((ctx.Link EndPoint.ImageUpload) + "?success=false")
namespace camblms.server.controller

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI

open camblms.dto
open camblms.server.service
open camblms.server.routing

module DocumentController =

    [<Rpc>]
    let getUsersWithValidDocuments sid =
        async {
            try
                let hasPermission = PermissionService.checkPermission sid Permissions.DocAdmin

                if not hasPermission then
                    return Error "Nem vagy jogosult lekérni az iratokat!"
                else
                    return DocumentService.getUsersWithValidDocuments ()
            with e ->
                return Error e.Message
        }

    [<Rpc>]
    let deleteImage (sid, filename) =
        async {
            try
                let hasPermission =
                    PermissionService.checkPermission sid Permissions.ServiceFeeAdmin

                if not hasPermission then
                    return InsufficientPermissions
                else
                    do DocumentService.deleteDocument filename
                    return ActionResult.Success
            with e ->
                return OtherError e.Message
        }

    [<Rpc>]
    let getImageList sid =
        async {
            try
                let hasPermission =
                    PermissionService.checkPermission sid Permissions.ServiceFeeAdmin

                if not hasPermission then
                    return Error "Nincs jogosultságod a szervízképekhez!"
                else
                    return Ok <| DocumentService.getImageList ()
            with e ->
                return Error e.Message
        }

    let uploadDocuments (ctx: Context<EndPoint>) (user: Member) =
        try
            let (personal, license) =
                (query {
                    for file in ctx.Request.Files do
                        where (file.Key = "personal")
                        exactlyOne
                 },
                 query {
                     for file in ctx.Request.Files do
                         where (file.Key = "licence")
                         exactlyOne
                 })

            do
                DocumentService.uploadDocuments
                    (personal.InputStream, personal.FileName)
                    (license.InputStream, license.FileName)
                    user

            Content.RedirectTemporaryToUrl((ctx.Link EndPoint.Documents) + "?success=true")
        with _ ->
            Content.RedirectTemporaryToUrl((ctx.Link EndPoint.Documents) + "?success=false")

    let uploadServiceImage (ctx: Context<EndPoint>) (user: Member) =
        try
            let file = Seq.item 0 ctx.Request.Files
            let isAactive = InactivityService.getActiveStatus user

            if not isAactive then
                Content.RedirectTemporaryToUrl((ctx.Link EndPoint.ImageUpload) + "?success=inactivity")
            else
                do DocumentService.uploadServiceImage file.InputStream file.FileName user
                Content.RedirectTemporaryToUrl((ctx.Link EndPoint.ImageUpload) + "?success=true")
        with _ ->
            Content.RedirectTemporaryToUrl((ctx.Link EndPoint.ImageUpload) + "?success=error")

    let getDocument (ctx: Context<EndPoint>) fileName =
        match ctx.Request.Cookies.Item "clms_sid" with
        | None -> Content.RedirectTemporary(EndPoint.Home)
        | Some s ->
            let hasPermission = PermissionService.checkPermission s Permissions.DocAdmin

            if hasPermission then
                if System.IO.File.Exists("docs/" + fileName) then
                    Content.File("../docs/" + fileName, true)
                else
                    Content.NotFound
            else
                let user = UserService.getUserFromSID s

                match user with
                | None -> Content.RedirectTemporary(EndPoint.Home)
                | Some u ->
                    if System.String(fileName).Split('_')[0] = string u.AccountID then
                        if System.IO.File.Exists("docs/" + fileName) then
                            Content.File("../docs/" + fileName, true)
                        else
                            Content.NotFound
                    else
                        Content.Forbidden

    let getServiceImage (ctx: Context<EndPoint>) fileName =
        match ctx.Request.Cookies.Item "clms_sid" with
        | None -> Content.RedirectTemporary(EndPoint.Home)
        | Some s ->
            let hasPermission = PermissionService.checkPermission s Permissions.ServiceFeeAdmin

            if hasPermission then
                if System.IO.File.Exists("img/" + fileName) then
                    Content.File("../img/" + fileName, true)
                else
                    Content.NotFound
            else
                Content.Forbidden

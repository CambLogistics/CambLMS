namespace camblms.server.routing

open WebSharper.Sitelets
open WebSharper.UI.Server
open WebSharper.UI.Html

open camblms.dto
open camblms.server.controller
open camblms.server.service

module Routing =
    let LoggedInRoute (user: Member) (ctx: Context<EndPoint>) endpoint =
        let sessionID = (ctx.Request.Cookies.Item "clms_sid").Value
        let name = UserService.getFirstName user
        UserService.lengthenSession sessionID

        let hasPermission =
            PermissionService.checkPermission sessionID (Map.find endpoint PermissionMap.RequiredPermissions)

        if not hasPermission then
            Content.RedirectTemporary(EndPoint.Home)
        else
            match endpoint with
            | EndPoint.Home -> Content.Page(PageMakers.Information ctx EndPoint.Information name)
            | EndPoint.Logout -> Content.Page(PageMakers.Logout ctx)
            | EndPoint.PasswordChange -> Content.Page(PageMakers.PasswordChange ctx endpoint name)
            | EndPoint.NameChange -> Content.Page(PageMakers.NameChange ctx endpoint name)
            | EndPoint.Changelog -> Content.Page(PageMakers.Changelog ctx endpoint name)
            | EndPoint.AdminHome -> Content.Page(PageMakers.AdminHome ctx endpoint name)
            | EndPoint.CarsAdmin -> Content.Page(PageMakers.CarsAdmin ctx endpoint name)
            | EndPoint.Inactivity -> Content.Page(PageMakers.InactivityPage ctx endpoint name)
            | EndPoint.Settings -> Content.Page(PageMakers.SettingsPage ctx endpoint name user)
            | EndPoint.InactivityAdmin -> Content.Page(PageMakers.InactivityAdmin ctx endpoint name)
            | EndPoint.Documents -> Content.Page(PageMakers.DocumentPage ctx endpoint name user)
            | EndPoint.DocumentSubmit -> DocumentController.uploadDocuments ctx user
            | EndPoint.ImageSubmit -> DocumentController.uploadServiceImage ctx user
            | EndPoint.ImageUpload -> Content.Page(PageMakers.ImageUpload ctx endpoint name)
            | EndPoint.Information -> Content.Page(PageMakers.Information ctx endpoint name)
            | EndPoint.MembersAdmin -> Content.Page(PageMakers.MembersAdmin ctx endpoint user name)
            | EndPoint.RegistrationAdmin -> Content.Page(PageMakers.RegistrationAdmin ctx endpoint name)
            | EndPoint.ServiceAdmin -> Content.Page(PageMakers.ServiceAdmin ctx endpoint name)
            | EndPoint.DocAdmin -> Content.Page(PageMakers.DocAdmin ctx endpoint name)
            | EndPoint.Taxi -> Content.Page(PageMakers.Taxi ctx endpoint name)
            | EndPoint.Towing -> Content.Page(PageMakers.Towing ctx endpoint name)
            | _ -> Content.RedirectTemporary(EndPoint.Information)

    let LoggedOutRoute (ctx: Context<EndPoint>) endpoint =
        match endpoint with
        | EndPoint.Home -> Content.Page(PageMakers.LoginPage ctx)
        | EndPoint.Login -> Content.Page(PageMakers.LoginPage ctx)
        | EndPoint.ForgotPass -> Content.Page(PageMakers.ForgotPass ctx)
        | _ -> Content.RedirectTemporary(EndPoint.Login)

    let MakeRoute (ctx: Context<EndPoint>) endpoint =
        async {
            match endpoint with
            | NotFound _ -> return! Content.Page(PageMakers.NotFound ctx)
            | DocServe fn -> return! DocumentController.getDocument ctx fn
            | ImgServe fn -> return! DocumentController.getServiceImage ctx fn
            | _ ->
                match ctx.Request.Cookies.Item "clms_sid" with
                | Some sid ->
                    let user = UserService.getUserFromSID sid

                    match user with
                    | Some u -> return! LoggedInRoute u ctx endpoint
                    | None -> return! LoggedOutRoute ctx endpoint
                | None -> return! LoggedOutRoute ctx endpoint
        }

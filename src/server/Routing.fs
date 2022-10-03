namespace camblms

open WebSharper.Sitelets
open WebSharper.UI.Server
open WebSharper.UI.Html

module Routing =
    let LoggedInRoute (user:Member) (ctx:Context<EndPoint>) endpoint =
        let sessionID = (ctx.Request.Cookies.Item "clms_sid").Value
        User.lengthenSession sessionID
        if not (Permission.checkPermission sessionID (Map.find endpoint Permission.RequiredPermissions)) then Content.RedirectTemporary(EndPoint.Home)
        else
        match endpoint with
            |EndPoint.Home -> Content.Page(PageMakers.Information ctx)
            |EndPoint.Logout -> Content.Page(PageMakers.Logout ctx)
            |EndPoint.PasswordChange -> Content.Page(PageMakers.PasswordChange ctx)
            |EndPoint.NameChange -> Content.Page(PageMakers.NameChange ctx)
            |EndPoint.Changelog -> Content.Page(PageMakers.Changelog ctx)
            |EndPoint.AdminHome -> Content.Page(PageMakers.AdminHome ctx)
            |EndPoint.CallsAdmin -> Content.Page(PageMakers.CallsAdmin ctx)
            |EndPoint.CarsAdmin -> Content.Page(PageMakers.CarsAdmin ctx)
            |EndPoint.Inactivity -> Content.Page(PageMakers.InactivityPage ctx)
            |EndPoint.InactivityAdmin -> Content.Page(PageMakers.InactivityAdmin ctx)
            |EndPoint.Documents -> Content.Page(Documents.MakePage ctx)
            |EndPoint.DocumentSubmit -> ImageSubmitter.Documents ctx user
            |EndPoint.ImageSubmit -> ImageSubmitter.Images ctx user
            |EndPoint.ImageUpload -> Content.Page(ImageUpload.MakePage ctx)
            |EndPoint.Information -> Content.Page(PageMakers.Information ctx)
            |EndPoint.MembersAdmin -> Content.Page(PageMakers.MembersAdmin ctx user)
            |EndPoint.NameChangeAdmin -> Content.Page(PageMakers.NameChangeAdmin ctx)
            |EndPoint.RegistrationAdmin -> Content.Page(PageMakers.RegistrationAdmin ctx)
            |EndPoint.ServiceAdmin -> Content.Page(PageMakers.ServiceAdmin ctx)
            |EndPoint.DocAdmin -> Content.Page(PageMakers.DocAdmin ctx)
            |EndPoint.ImgAdmin -> Content.Page(PageMakers.ImgAdmin ctx)
            |EndPoint.Taxi -> Content.Page(PageMakers.Taxi ctx)
            |EndPoint.Towing -> Content.Page(PageMakers.Towing ctx)
            |_ -> Content.RedirectTemporary(EndPoint.Information)
    let LoggedOutRoute (ctx:Context<EndPoint>) endpoint =
        match endpoint with
            |EndPoint.Home -> Content.Page(PageMakers.LoginPage ctx)
            |EndPoint.Login -> Content.Page(PageMakers.LoginPage ctx)
            |EndPoint.Registration -> Content.Page(PageMakers.RegisterPage ctx)
            |EndPoint.ForgotPass -> Content.Page(PageMakers.ForgotPass ctx)
            |_ -> Content.RedirectTemporary(EndPoint.Login)

    let MakeRoute (ctx:Context<EndPoint>) endpoint =
        match endpoint with
            |NotFound _ -> Content.Page(PageMakers.NotFound ctx)
            |DocServe fn -> ImageServe.Docs ctx fn
            |ImgServe fn -> ImageServe.Service ctx fn
            | _ ->
                match ctx.Request.Cookies.Item "clms_sid" with
                    |Some sid -> 
                        let user = User.getUserFromSID sid
                        match user with
                            |Some u -> LoggedInRoute u ctx endpoint
                            |None -> LoggedOutRoute ctx endpoint
                    |None -> LoggedOutRoute ctx endpoint

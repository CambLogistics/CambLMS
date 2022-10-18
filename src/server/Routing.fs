namespace camblms

open WebSharper.Sitelets
open WebSharper.UI.Server
open WebSharper.UI.Html

module Routing =
    let LoggedInRoute (user:Member) (ctx:Context<EndPoint>) endpoint =
        let sessionID = (ctx.Request.Cookies.Item "clms_sid").Value
        let name = User.getFirstName user
        User.lengthenSession sessionID
        if not (Permission.checkPermission sessionID (Map.find endpoint Permission.RequiredPermissions)) then Content.RedirectTemporary(EndPoint.Home)
        else
        match endpoint with
            |EndPoint.Home -> Content.Page(PageMakers.Information ctx endpoint name)
            |EndPoint.Logout -> Content.Page(PageMakers.Logout ctx)
            |EndPoint.PasswordChange -> Content.Page(PageMakers.PasswordChange ctx endpoint name)
            |EndPoint.NameChange -> Content.Page(PageMakers.NameChange ctx endpoint name)
            |EndPoint.Changelog -> Content.Page(PageMakers.Changelog ctx endpoint name)
            |EndPoint.AdminHome -> Content.Page(PageMakers.AdminHome ctx endpoint name)
            |EndPoint.CallsAdmin -> Content.Page(PageMakers.CallsAdmin ctx endpoint name)
            |EndPoint.CarsAdmin -> Content.Page(PageMakers.CarsAdmin ctx endpoint name)
            |EndPoint.Inactivity -> Content.Page(PageMakers.InactivityPage ctx endpoint name)
            |EndPoint.InactivityAdmin -> Content.Page(PageMakers.InactivityAdmin ctx endpoint name)
            |EndPoint.Documents -> Content.Page(Documents.MakePage ctx endpoint name)
            |EndPoint.DocumentSubmit -> ImageSubmitter.Documents ctx user
            |EndPoint.ImageSubmit -> ImageSubmitter.Images ctx user
            |EndPoint.ImageUpload -> Content.Page(ImageUpload.MakePage ctx endpoint name)
            |EndPoint.Information -> Content.Page(PageMakers.Information ctx endpoint name)
            |EndPoint.MembersAdmin -> Content.Page(PageMakers.MembersAdmin ctx endpoint user name)
            |EndPoint.NameChangeAdmin -> Content.Page(PageMakers.NameChangeAdmin ctx endpoint name)
            |EndPoint.RegistrationAdmin -> Content.Page(PageMakers.RegistrationAdmin ctx endpoint name)
            |EndPoint.ServiceAdmin -> Content.Page(PageMakers.ServiceAdmin ctx endpoint name)
            |EndPoint.DocAdmin -> Content.Page(PageMakers.DocAdmin ctx endpoint name)
            |EndPoint.ImgAdmin -> Content.Page(PageMakers.ImgAdmin ctx endpoint name)
            |EndPoint.Taxi -> Content.Page(PageMakers.Taxi ctx endpoint name)
            |EndPoint.Towing -> Content.Page(PageMakers.Towing ctx endpoint name)
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

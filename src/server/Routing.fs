namespace camblms

open WebSharper.Sitelets
open WebSharper.UI.Server
open WebSharper.UI.Html

module Routing =
    let LoggedInRoute (user:Member) (ctx:Context<EndPoint>) endpoint =
        let (minRole,maxRole) = Map.find endpoint EndPoints.PermissionList
        let sessionID = (ctx.Request.Cookies.Item "clms_sid").Value
        if user.Role < minRole || 
            (user.Role > maxRole && (User.verifyAdmin (sessionID) |> not)) 
            || minRole = -2 then
                Content.RedirectPermanent(EndPoint.Information)
        else
            match endpoint with
                |EndPoint.Home -> Content.Page(PageMakers.Information ctx)
                |EndPoint.Logout -> User.makeLogout sessionID ctx
                |EndPoint.PasswordChange -> Content.Page(PageMakers.PasswordChange ctx)
                |EndPoint.NameChange -> Content.Page(PageMakers.NameChange ctx)
                |EndPoint.Changelog -> Content.Page(PageMakers.Changelog ctx)
                |EndPoint.AdminHome -> Content.Page(PageMakers.AdminHome ctx)
                |EndPoint.CallsAdmin -> Content.Page(PageMakers.CallsAdmin ctx)
                |EndPoint.CarsAdmin -> Content.Page(PageMakers.CarsAdmin ctx)
                |EndPoint.Delivery -> Content.Page(PageMakers.Delivery ctx)
                |EndPoint.Documents -> Content.Page(Documents.MakePage ctx)
                |EndPoint.DocumentSubmit -> ImageSubmitter.Documents ctx user
                |EndPoint.ImageSubmit -> ImageSubmitter.Images ctx user
                |EndPoint.ImageUpload -> Content.Page(ImageUpload.MakePage ctx)
                |EndPoint.Information -> Content.Page(PageMakers.Information ctx)
                |EndPoint.MembersAdmin -> Content.Page(PageMakers.MembersAdmin ctx user)
                |EndPoint.RegistrationAdmin -> Content.Page(PageMakers.RegistrationAdmin ctx)
                |EndPoint.ServiceAdmin -> Content.Page(PageMakers.ServiceAdmin ctx)
                |EndPoint.Taxi -> Content.Page(PageMakers.Taxi ctx)
                |EndPoint.Towing -> Content.Page(PageMakers.Towing ctx)

    let LoggedOutRoute (ctx:Context<EndPoint>) endpoint =
        let neededPermission,_ = Map.find endpoint EndPoints.PermissionList
        if neededPermission > -1 then Content.RedirectPermanent(EndPoint.Login)
        else
            match endpoint with
                |EndPoint.Home -> Content.Page(PageMakers.LoginPage ctx)
                |EndPoint.Login -> Content.Page(PageMakers.LoginPage ctx)
                |EndPoint.Registration -> Content.Page(PageMakers.RegisterPage ctx)

    let MakeRoute (ctx:Context<EndPoint>) endpoint =
        match ctx.Request.Cookies.Item "clms_sid" with
            |Some sid -> 
                let user = User.getUserFromSID sid
                match user with
                    |Some u -> LoggedInRoute u ctx endpoint
                    |None -> LoggedOutRoute ctx endpoint
            |None -> LoggedOutRoute ctx endpoint

namespace camblms

open WebSharper.Sitelets
open WebSharper.UI.Server
open WebSharper.UI.Html

module Routing =
    let LoggedInRoute (user:Member) (ctx:Context<EndPoint>) endpoint =
        let (minRole,maxRole) = Map.find endpoint EndPoints.PermissionList
        let sessionID = (ctx.Request.Cookies.Item "clms_sid").Value
        if user.Role < minRole || (user.Role > maxRole && User.verifyAdmin (sessionID) |> not) || neededPermission = -2 then
            Content.RedirectPermanent(EndPoint.Information)
        else
            match endpoint with
                |EndPoint.Home -> Content.Page(Information.MakePage ctx)
                |EndPoint.Logout -> User.makeLogout sessionID ctx
                |EndPoint.PasswordChange -> Content.Page(PageMakers.PasswordChange ctx)
                |EndPoint.NameChange -> Content.Page(PageMakers.NameChange ctx)
                |EndPoint.Changelog -> Content.Page(PageMakers.Changelog ctx)
                |EndPoint.AdminHome -> Content.Page(AdminHome.MakePage ctx)
                |EndPoint.CallsAdmin -> Content.Page(CallsAdmin.MakePage ctx)
                |EndPoint.CarsAdmin -> Content.Page(CarsAdmin.MakePage ctx)
                |EndPoint.Delivery -> Content.Page(Delivery.MakePage ctx)
                |EndPoint.Documents -> Content.Page(Documents.MakePage ctx)
                |EndPoint.DocumentSubmit -> ImageSubmitter.Documents ctx user
                |EndPoint.ImageSubmit -> ImageSubmitter.Images ctx user
                |EndPoint.ImageUpload -> Content.Page(ImageUpload.MakePage ctx)
                |EndPoint.Information -> Content.Page(Information.MakePage ctx)
                |EndPoint.MembersAdmin -> Content.Page(PageMakers.MembersAdmin ctx user)
                |EndPoint.RegistrationAdmin -> Content.Page(PageMakers.RegistrationAdmin ctx)
                |EndPoint.ServiceAdmin -> Content.Page(ServiceAdmin.MakePage ctx)
                |EndPoint.Taxi -> Content.Page(Taxi.MakePage ctx)
                |EndPoint.Towing -> Content.Page(Towing.MakePage ctx)

    let LoggedOutRoute (ctx:Context<EndPoint>) endpoint =
        let neededPermission = Map.find endpoint EndPoints.PermissionList
        if neededPermission > -1 then Content.RedirectPermanent(EndPoint.Login)
        else
            match endpoint with
                |EndPoint.Home -> Content.Page(client <@LoginPage.RenderPage@>)
                |EndPoint.Login -> Content.Page(client <@LoginPage.RenderPage@>)
                |EndPoint.Registration -> Content.Page(client <@RegistrationPage.RenderPage@>)

    let MakeRoute (ctx:Context<EndPoint>) endpoint =
        match ctx.Request.Cookies.Item "clms_sid" with
            |Some sid -> 
                let user = User.getUserFromSID sid
                match user with
                    |Some u -> LoggedInRoute u ctx endpoint
                    |None -> LoggedOutRoute ctx endpoint
            |None -> LoggedOutRoute ctx endpoint

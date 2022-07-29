namespace camblms

open WebSharper.Sitelets
open WebSharper.UI.Server
open WebSharper.UI.Html

module Routing =
    let LoggedInRoute (user:Member) (ctx:Context<EndPoint>) endpoint =
        let sessionID = (ctx.Request.Cookies.Item "clms_sid").Value
        User.lengthenSession sessionID
        let neededPermissions = Map.find endpoint Permission.RequiredPermissions
        let needsAdmin = neededPermissions > ~~~Permissions.Admin && neededPermissions <> Permissions.Nothing
        if not (Permission.checkPermission sessionID neededPermissions) then Content.RedirectTemporary(EndPoint.Home)
        else
        match endpoint with
            |EndPoint.Home -> Content.Page(PageMakers.NormalPage ctx true false Information.RenderPage ctx)
            |EndPoint.Logout -> 
                    Content.Page(
                        (fun () -> 
                            User.logoutUser ctx
                            PageMakers.NormalPage ctx false false LogoutClient.Logout ())()
                    )
            |EndPoint.PasswordChange -> Content.Page(PageMakers.NormalPage ctx false needsAdmin PasswordChangePage.RenderPage ())
            |EndPoint.NameChange -> Content.Page(PageMakers.NormalPage ctx false needsAdmin NameChangePage.RenderPage ())
            |EndPoint.Changelog -> Content.Page(PageMakers.NormalPage ctx true needsAdmin (fun () -> SiteParts.ChangelogTemplate().Doc()) ())
            |EndPoint.CallsAdmin -> Content.Page(PageMakers.NormalPage ctx false needsAdmin CallsAdmin.RenderPage ())
            |EndPoint.CarsAdmin -> Content.Page(PageMakers.NormalPage ctx false needsAdmin CarsAdmin.RenderPage ())
            |EndPoint.Inactivity -> Content.Page(PageMakers.NormalPage ctx false needsAdmin InactivityPage.RenderPage ())
            |EndPoint.Delivery -> Content.Page(PageMakers.NormalPage ctx false needsAdmin DeliveryPage.RenderPage ())
            |EndPoint.Documents -> Content.Page(Documents.MakePage ctx)
            |EndPoint.DocumentSubmit -> ImageSubmitter.Documents ctx user
            |EndPoint.ImageSubmit -> ImageSubmitter.Images ctx user
            |EndPoint.ImageUpload -> Content.Page(ImageUpload.MakePage ctx)
            |EndPoint.Information -> Content.Page(PageMakers.NormalPage ctx true needsAdmin Information.RenderPage ctx)
            |EndPoint.MembersAdmin -> Content.Page(PageMakers.NormalPage ctx false needsAdmin MemberAdminPage.RenderPage user)
            |EndPoint.InactivityAdmin -> Content.Page(PageMakers.NormalPage ctx false needsAdmin InactivityAdmin.RenderPage ())
            |EndPoint.NameChangeAdmin -> Content.Page(PageMakers.NormalPage ctx false needsAdmin NameChangeAdmin.RenderPage ())
            |EndPoint.RegistrationAdmin -> Content.Page(PageMakers.NormalPage ctx false needsAdmin RegistrationAdminPage.RenderPage ())
            |EndPoint.ServiceAdmin -> Content.Page(PageMakers.NormalPage ctx false needsAdmin ServiceFeeAdmin.RenderPage ())
            |EndPoint.DocAdmin -> Content.Page(PageMakers.NormalPage ctx false needsAdmin DocAdmin.RenderPage ())
            |EndPoint.ImgAdmin -> Content.Page(PageMakers.NormalPage ctx false needsAdmin ImageAdmin.RenderPage ())
            |EndPoint.Taxi -> Content.Page(PageMakers.NormalPage ctx false needsAdmin TaxiPage.RenderPage ())
            |EndPoint.Towing -> Content.Page(PageMakers.NormalPage ctx false needsAdmin TowPage.RenderPage ())
            |_ -> Content.RedirectTemporary(EndPoint.Information)
    let LoggedOutRoute (ctx:Context<EndPoint>) endpoint =
        match endpoint with
            |EndPoint.Home -> Content.Page(PageMakers.LoginPage ctx LoginPage.RenderPage ())
            |EndPoint.Login -> Content.Page(PageMakers.LoginPage ctx LoginPage.RenderPage ())
            |EndPoint.Registration -> Content.Page(PageMakers.LoginPage ctx RegistrationPage.RenderPage ())
            |EndPoint.ForgotPass -> Content.Page(PageMakers.LoginPage ctx ForgotPassPage.RenderPage ())
            |_ -> Content.RedirectTemporary(EndPoint.Login)

    let MakeRoute (ctx:Context<EndPoint>) endpoint =
        match endpoint with
            |NotFound _ -> Content.Page(PageMakers.NormalPage ctx true false (fun () -> SiteParts.NotFoundTemplate().ErrorMessage("A keresett oldal nem található!").Doc()) ())
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

namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.Sitelets

module PageMakers =
    let RegistrationAdmin ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.RegistrationAdmin)
            .Main(client <@RegistrationAdminPage.RenderPage()@>)
            .Doc()
    let PasswordChange ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.ChangeStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.PasswordChange)
            .Main(client <@PasswordChangePage.RenderPage()@>)
            .Doc()
    let NameChange ctx =
         SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.ChangeStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.NameChange)
            .Main(client <@NameChangePage.RenderPage()@>)
            .Doc()
    let MembersAdmin ctx user =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.MembersAdmin)
            .Main(client <@MemberAdminPage.RenderPage user@>)
            .Doc()
    let Changelog ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.Changelog)
            .Main(SiteParts.ChangelogTemplate().Doc())
            .Doc()
    let CarsAdmin ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.CarsAdmin)
            .Main(client <@CarsAdmin.RenderPage()@>)
            .Doc()
    let Information ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.Information)
            .Main(Information.RenderPage ctx)
            .Doc()
    let Taxi ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.Taxi)
            .Main(client <@TaxiPage.RenderPage()@>)
            .Doc()
    let Towing ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.Towing)
            .Main(client <@TowPage.RenderPage()@>)
            .Doc()
    let Delivery ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.Delivery)
            .Main()
            .Doc()
    let AdminHome ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.AdminHome)
            .Main(SiteParts.AdminHomeTemplate().Doc())
            .Doc()
    let CallsAdmin ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.CallsAdmin)
            .Main(client <@CallsAdmin.RenderPage()@>)
            .Doc()
    let ServiceAdmin ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.ServiceAdmin)
            .Main()
            .Doc()
    let LoginPage ctx =
        SiteTemplates.LoginTemplate()
            .Main(client <@LoginPage.RenderPage()@>)
            .Doc()
    let RegisterPage ctx =
        SiteTemplates.LoginTemplate()
            .Main(client <@RegistrationPage.RenderPage()@>)
            .Doc()
    let NameChangeAdmin ctx =
        SiteTemplates.MainTemplate()   
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.NameChangeAdmin)
            .Main(client <@NameChangeAdmin.RenderPage()@>)
            .Doc()
    let Logout (ctx:Context<EndPoint>) =
        User.logoutUser ctx
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar()
            .Main(client <@LogoutClient.Logout()@>)
            .Doc()
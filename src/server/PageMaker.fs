namespace camblms

open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.Sitelets

module PageMakers =
    let RegistrationAdmin ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.RegistrationAdmin)
            .Main(client <@RegistrationAdminPage.RenderPage@>)
            .Doc()
    let PasswordChange ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.ChangeStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.PasswordChange)
            .Main(client <@PasswordChangePage.RenderPage@>)
            .Doc()
    let NameChange ctx =
         SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.ChangeStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.NameChange)
            .Main(client <@NameChangePage.RenderPage@>)
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
            .Navbar(Navbar.MakeNavbar ctx EndPoint.MembersAdmin)
            .Main(SiteParts.ChangelogTemplate().Doc())
            .Doc()
    let CarsAdmin ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.CarsAdmin)
            .Main(client <@CarsAdmin.RenderPage@>)
            .Doc()
    let Information ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.Information)
            .Main()
            .Doc()
    let Taxi ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.Taxi)
            .Main()
            .Doc()
    let Towing ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.Towing)
            .Main()
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
            .Main()
            .Doc()
    let CallsAdmin ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.CallsAdmin)
            .Main()
            .Doc()
    let ServiceAdmin ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx EndPoint.ServiceAdmin)
            .Main()
            .Doc()
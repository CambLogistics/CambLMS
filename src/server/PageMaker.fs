namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.Sitelets

module PageMakers =
    let RegistrationAdmin ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx true)
            .Main(client <@RegistrationAdminPage.RenderPage()@>)
            .Doc()
    let PasswordChange ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.ChangeStyle)
            .Navbar(Navbar.MakeNavbar ctx false)
            .Main(client <@PasswordChangePage.RenderPage()@>)
            .Doc()
    let NameChange ctx =
         SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.ChangeStyle)
            .Navbar(Navbar.MakeNavbar ctx false)
            .Main(client <@NameChangePage.RenderPage()@>)
            .Doc()
    let MembersAdmin ctx user =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx true)
            .Main(client <@MemberAdminPage.RenderPage user@>)
            .Doc()
    let Changelog ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx false)
            .Main(SiteParts.ChangelogTemplate().Doc())
            .Doc()
    let CarsAdmin ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx true)
            .Main(client <@CarsAdmin.RenderPage()@>)
            .Doc()
    let Information ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx false)
            .Main(Information.RenderPage ctx)
            .Doc()
    let Taxi ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx false)
            .Main(client <@TaxiPage.RenderPage()@>)
            .Doc()
    let Towing ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx false)
            .Main(client <@TowPage.RenderPage()@>)
            .Doc()
    let Delivery ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx false)
            .Main(client <@DeliveryPage.RenderPage()@>)
            .Doc()
    let AdminHome ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx true)
            .Main(Doc.Empty)
            .Doc()
    let CallsAdmin ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx true)
            .Main(client <@CallsAdmin.RenderPage()@>)
            .Doc()
    let ServiceAdmin ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx true)
            .Main(client <@ServiceFeeAdmin.RenderPage()@>)
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
            .Navbar(Navbar.MakeNavbar ctx true)
            .Main(client <@NameChangeAdmin.RenderPage()@>)
            .Doc()
    let DocAdmin ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx true)
            .Main(client <@DocAdmin.RenderPage()@>)
            .Doc()
    let ImgAdmin ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.AdminStyle)
            .Navbar(Navbar.MakeNavbar ctx true)
            .Main(client <@ImageAdmin.RenderPage()@>)
            .Doc()
    let Logout (ctx:Context<EndPoint>) =
        User.logoutUser ctx
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar()
            .Main(client <@LogoutClient.Logout()@>)
            .Doc()
    let NotFound ctx =
        SiteTemplates.MainTemplate()
            .Stylesheet(SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx false)
            .Main(SiteParts.NotFoundTemplate().ErrorMessage("A keresett oldal nem található!").Doc())
            .Doc()


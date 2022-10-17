namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.Sitelets

module PageMakers =
    let RegistrationAdmin ctx (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx true)
            .FirstName(name)
            .Main(client <@RegistrationAdminPage.RenderPage()@>)
            .Doc()
    let PasswordChange ctx (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx false)
            .FirstName(name)
            .Main(client <@PasswordChangePage.RenderPage()@>)
            .Doc()
    let NameChange ctx (name:string) =
         SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx false)
            .FirstName(name)
            .Main(client <@NameChangePage.RenderPage()@>)
            .Doc()
    let MembersAdmin ctx user (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx true)
            .FirstName(name)
            .Main(client <@MemberAdminPage.RenderPage user@>)
            .Doc()
    let Changelog ctx (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx false)
            .FirstName(name)
            .Main(SiteParts.ChangelogTemplate().Doc())
            .Doc()
    let CarsAdmin ctx (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx true)
            .FirstName(name)
            .Main(client <@CarsAdmin.RenderPage()@>)
            .Doc()
    let Information ctx (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx false)
            .FirstName(name)
            .Main(Information.RenderPage ctx)
            .Doc()
    let Taxi ctx (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx false)
            .FirstName(name)
            .Main(client <@TaxiPage.RenderPage()@>)
            .Doc()
    let Towing ctx (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx false)
            .FirstName(name)
            .Main(client <@TowPage.RenderPage()@>)
            .Doc()
    let AdminHome ctx (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx true)
            .FirstName(name)
            .Main(Doc.Empty)
            .Doc()
    let CallsAdmin ctx (name:string) =
        SiteTemplates.MainTemplate()
            .FirstName(name)
            .Navbar(Navbar.MakeNavbar ctx true)
            .Main(client <@CallsAdmin.RenderPage()@>)
            .Doc()
    let ServiceAdmin ctx (name:string) =
        SiteTemplates.MainTemplate()
            .FirstName(name)
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
    let InactivityPage ctx (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx false)
            .FirstName(name)
            .Main(client <@InactivityPage.RenderPage()@>)
            .Doc()
    let InactivityAdmin ctx (name:string) =
        SiteTemplates.MainTemplate()   
            .Navbar(Navbar.MakeNavbar ctx true)
            .FirstName(name)
            .Main(client <@InactivityAdmin.RenderPage()@>)
            .Doc()
    let NameChangeAdmin ctx (name:string) =
        SiteTemplates.MainTemplate()
            .FirstName(name) 
            .Navbar(Navbar.MakeNavbar ctx true)
            .Main(client <@NameChangeAdmin.RenderPage()@>)
            .Doc()
    let DocAdmin ctx (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx true)
            .FirstName(name)
            .Main(client <@DocAdmin.RenderPage()@>)
            .Doc()
    let ImgAdmin ctx (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx true)
            .FirstName(name)
            .Main(client <@ImageAdmin.RenderPage()@>)
            .Doc()
    let ForgotPass ctx =
        SiteTemplates.LoginTemplate()
            .Main(client <@ForgotPassPage.RenderPage()@>)
            .Doc()
    let Logout (ctx:Context<EndPoint>) =
        User.logoutUser ctx
        SiteTemplates.MainTemplate()
            .Navbar()
            .Main(client <@LogoutClient.Logout()@>)
            .Doc()
    let NotFound ctx =
        SiteTemplates.MainTemplate()
            .FirstName("látogató")
            .Navbar()
            .Main(SiteParts.NotFoundTemplate().ErrorMessage("A keresett oldal nem található!").Doc())
            .Doc()


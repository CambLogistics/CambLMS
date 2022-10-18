namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.Sitelets

module PageMakers =
    let RegistrationAdmin ctx ep (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx ep true)
            .FirstName(name)
            .Main(client <@RegistrationAdminPage.RenderPage()@>)
            .Doc()
    let PasswordChange ctx ep (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx ep false)
            .FirstName(name)
            .Main(client <@PasswordChangePage.RenderPage()@>)
            .Doc()
    let NameChange ctx ep (name:string) =
         SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx ep false)
            .FirstName(name)
            .Main(client <@NameChangePage.RenderPage()@>)
            .Doc()
    let MembersAdmin ctx ep user (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx ep true)
            .FirstName(name)
            .Main(client <@MemberAdminPage.RenderPage user@>)
            .Doc()
    let Changelog ctx ep (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx ep false)
            .FirstName(name)
            .Main(SiteParts.ChangelogTemplate().Doc())
            .Doc()
    let CarsAdmin ctx ep (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx ep true)
            .FirstName(name)
            .Main(client <@CarsAdmin.RenderPage()@>)
            .Doc()
    let Information ctx ep (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx ep false)
            .FirstName(name)
            .Main(Information.RenderPage ctx)
            .Doc()
    let Taxi ctx ep (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx ep false)
            .FirstName(name)
            .Main(client <@TaxiPage.RenderPage()@>)
            .Doc()
    let Towing ctx ep (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx ep false)
            .FirstName(name)
            .Main(client <@TowPage.RenderPage()@>)
            .Doc()
    let AdminHome ctx ep (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx ep true)
            .FirstName(name)
            .Main(Doc.Empty)
            .Doc()
    let CallsAdmin ctx ep (name:string) =
        SiteTemplates.MainTemplate()
            .FirstName(name)
            .Navbar(Navbar.MakeNavbar ctx ep true)
            .Main(client <@CallsAdmin.RenderPage()@>)
            .Doc()
    let ServiceAdmin ctx ep (name:string) =
        SiteTemplates.MainTemplate()
            .FirstName(name)
            .Navbar(Navbar.MakeNavbar ctx ep true)
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
    let InactivityPage ctx ep (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx ep false)
            .FirstName(name)
            .Main(client <@InactivityPage.RenderPage()@>)
            .Doc()
    let InactivityAdmin ctx ep (name:string) =
        SiteTemplates.MainTemplate()   
            .Navbar(Navbar.MakeNavbar ctx ep true)
            .FirstName(name)
            .Main(client <@InactivityAdmin.RenderPage()@>)
            .Doc()
    let NameChangeAdmin ctx ep (name:string) =
        SiteTemplates.MainTemplate()
            .FirstName(name) 
            .Navbar(Navbar.MakeNavbar ctx ep true)
            .Main(client <@NameChangeAdmin.RenderPage()@>)
            .Doc()
    let DocAdmin ctx ep (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx ep true)
            .FirstName(name)
            .Main(client <@DocAdmin.RenderPage()@>)
            .Doc()
    let ImgAdmin ctx ep (name:string) =
        SiteTemplates.MainTemplate()
            .Navbar(Navbar.MakeNavbar ctx ep true)
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


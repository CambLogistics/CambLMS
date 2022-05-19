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
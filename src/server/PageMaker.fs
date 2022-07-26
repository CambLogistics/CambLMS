namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.Sitelets

module PageMakers =
    let NormalPage ctx isServerSide isAdmin renderer passToRenderer =
        SiteTemplates.MainTemplate()
            .Stylesheet(if isAdmin then SiteTemplates.AdminStyle else SiteTemplates.NormalStyle)
            .Navbar(Navbar.MakeNavbar ctx isAdmin)
            .Main(if isServerSide then renderer passToRenderer else client <@renderer passToRenderer@>)
            .Doc()
    let LoginPage ctx renderer passtoRenderer =
        SiteTemplates.LoginTemplate()
                .Main(client <@renderer passtoRenderer@>)
                .Doc()

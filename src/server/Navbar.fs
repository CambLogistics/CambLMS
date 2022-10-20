namespace camblms

open SiteTemplates
open WebSharper.Sitelets
open WebSharper.UI

module Navbar =
    type NavbarElement =
        |Url of EndPoint*string*string
        |LogoutUrl of string
    let NormalNavbar = [
        Url (EndPoint.Information,"Információk","users")
        Url (EndPoint.Changelog,"Változásnapló","bell")
        Url (EndPoint.Taxi,"Taxizás","taxi")
        Url (EndPoint.Towing,"Vontatás","truck")
        Url (EndPoint.Documents,"Iratbeküldés","id-card")
        Url (EndPoint.ImageUpload,"Képfeltöltés","image")
        Url (EndPoint.Inactivity,"Inaktivítás","free-code-camp")
        Url (EndPoint.Settings,"Beállítások","cog")
        Url (EndPoint.AdminHome,"Adminfelület","shield")
        LogoutUrl "Kijelentkezés"
    ]
    let AdminNavbar = [
        Url (EndPoint.AdminHome,"Kezdőoldal","shield")
        Url (EndPoint.RegistrationAdmin,"Regisztrációk","user-plus")
        Url (EndPoint.DocAdmin,"Iratok","id-card")
        Url(EndPoint.MembersAdmin,"Tagok","users")
        Url(EndPoint.InactivityAdmin,"Inaktivítás","free-code-camp")
        Url(EndPoint.CarsAdmin,"Autók","car")
        Url(EndPoint.ServiceAdmin,"Szerviz","wrench")
        Url (EndPoint.Home,"Normál felület","taxi")
        LogoutUrl "Kijelentkezés"
    ]
    let MakeNavbar (ctx:Context<EndPoint>) current isAdmin =
        let user =
            match ctx.Request.Cookies.Item "clms_sid" with
                |Some s -> User.getUserFromSID s
                |None -> None
        let generateItems (ctx:Context<EndPoint>) elements =
            List.map(fun item ->
                match item with
                    |LogoutUrl name -> MainTemplate.NavbarLogout().Url(ctx.Link EndPoint.Logout).ItemTitle(name).Doc()
                    |Url (ep,name,icon) ->
                        if ep = current then MainTemplate.NavbarActiveItem().Url(ctx.Link ep).ItemTitle(name).IconClass(icon).Doc()
                        else MainTemplate.NavbarItem().Url(ctx.Link ep).ItemTitle(name).IconClass(icon).Doc()
            ) elements |> Doc.Concat
        let navbarList = if isAdmin then AdminNavbar else NormalNavbar
        navbarList |> List.filter (
            fun item ->
                match item with
                    |LogoutUrl _ -> user.IsSome
                    |Url (ep,_,_) ->
                        if user.IsSome then (Permission.getUserPermissions user.Value) &&& (LanguagePrimitives.EnumToValue (Map.find ep Permission.RequiredPermissions)) > 0u
                        else false
            ) |> generateItems ctx

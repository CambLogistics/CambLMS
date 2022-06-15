namespace camblms

open SiteParts
open WebSharper.Sitelets
open WebSharper.UI

module Navbar =
    type NavbarElement =
        |Url of EndPoint*string
        |Separator
        |Logo
    let NormalNavbar = [
        Logo
        Separator
        Url (EndPoint.Information,"Információk")
        Url (EndPoint.Changelog,"Changelog")
        Separator
        Url (EndPoint.Taxi,"Taxizás")
        Url (EndPoint.Towing,"Vontatás")
        Url (EndPoint.Delivery,"Beszállítás")
        Separator
        Url (EndPoint.Documents,"Iratbeküldés")
        Url (EndPoint.ImageUpload,"Képfeltöltés")
        Separator
        Url (EndPoint.AdminHome,"Adminfelület")
        Url (EndPoint.Logout,"Kijelentkezés")
    ]
    let AdminNavbar = [
        Logo
        Separator
        Url (EndPoint.Home,"Normál felület")
        Separator
        Url (EndPoint.CallsAdmin,"Hívások")
        Url (EndPoint.NameChangeAdmin,"Névváltoztatások")
        Url (EndPoint.RegistrationAdmin,"Regisztációk")
        Url (EndPoint.DocAdmin,"Iratok")
        Separator
        Url(EndPoint.MembersAdmin,"Tagok")
        Url(EndPoint.CarsAdmin,"Autók")
        Url(EndPoint.ImgAdmin,"Szervizképek")
        Url(EndPoint.ServiceAdmin,"Szervizdíjak")
        Separator
        Url(EndPoint.Logout,"Kijelentkezés")
    ]
    let MakeNavbar (ctx:Context<EndPoint>) isAdmin =
        let navTemplate = NavTemplate()
        let user =
            match ctx.Request.Cookies.Item "clms_sid" with
                |Some s -> User.getUserFromSID s
                |None -> None
        let generateItems (ctx:Context<EndPoint>) elements =
            List.map(fun item ->
                match item with
                    |Logo -> NavTemplate.Logo().Doc()
                    |Separator -> NavTemplate.Separator().Doc()
                    |Url (ep,name) ->
                        NavTemplate.NavbarElement().EndpointURL(ctx.Link ep).EndpointName(name).Doc()
            ) elements |> Doc.Concat
        navTemplate.NavList(
            let navbarList = if isAdmin then AdminNavbar else NormalNavbar
            navbarList |> List.filter (
                fun item ->
                    match item with
                        |Logo -> true
                        |Url (ep,_) ->
                            if user.IsSome then (Permission.getUserPermissions user.Value) &&& (LanguagePrimitives.EnumToValue (Map.find ep Permission.RequiredPermissions)) > 0u
                            else false
                        |Separator -> user.IsSome
            ) |> generateItems ctx
        ).Doc()

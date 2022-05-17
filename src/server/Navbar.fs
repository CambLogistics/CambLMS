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
        Url (EndPoint.CallsAdmin,"Hívások")
        Url (EndPoint.NameChangeAdmin,"Névváltoztatások")
        Url (EndPoint.RegistrationAdmin,"Regisztációk")
        Separator
        Url(EndPoint.MembersAdmin,"Tagok")
        Url(EndPoint.CarsAdmin,"Autók")
        Url(EndPoint.ServiceAdmin,"Szervizdíjak")
    ]
    let MakeNavbar (ctx:Context<EndPoint>) endpoint =
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
        if Map.find endpoint EndPoints.PermissionList >= 12 then
            navTemplate.NavList(
            generateItems ctx AdminNavbar).Doc()
        else 
            navTemplate.NavList(
               NormalNavbar |> List.filter (
                   fun item ->
                    match item with
                        |Logo -> true
                        |Url (ep,name) ->
                            if user.IsSome then user.Value.Role >= Map.find ep EndPoints.PermissionList
                            else false
                        |Separator -> user.IsSome
               ) |> generateItems ctx
                ).Doc()

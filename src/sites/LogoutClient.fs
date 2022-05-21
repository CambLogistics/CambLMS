namespace camblms

open WebSharper
open WebSharper.UI

[<JavaScript>]
module LogoutClient =
    let Logout() =
        JavaScript.Cookies.Expire("clms_sid")
        JavaScript.JS.Window.Location.Replace "/"
        Doc.Empty
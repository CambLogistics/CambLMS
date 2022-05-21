namespace camblms

open WebSharper
open WebSharper.UI

module LogoutClient =
    let Logout() =
        JavaScript.Cookies.Expire("clms_sid")
        Doc.Empty
namespace camblms

open WebSharper
open WebSharper.Sitelets

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/leave">] Logout
    | [<EndPoint "/login">] Login
    | [<EndPoint "/registration">] Registration
    | [<EndPoint "/passchange">] PasswordChange
    | [<EndPoint "/namechange">] NameChange
    | [<EndPoint "/info">] Information
    | [<EndPoint "/delivery">] Delivery
    | [<EndPoint "/taxi">] Taxi
    | [<EndPoint "/tow">] Towing
    | [<EndPoint "/docs">] Documents
    | [<EndPoint "/img">] ImageUpload
    | [<EndPoint "/admin">] AdminHome
    | [<EndPoint "/admin/calls">] CallsAdmin
    | [<EndPoint "/admin/cars">] CarsAdmin
    | [<EndPoint "/admin/members">] MembersAdmin
    | [<EndPoint "/admin/regadmin">] RegistrationAdmin
    | [<EndPoint "/admin/nameadmin">] NameChangeAdmin
    | [<EndPoint "/admin/service">] ServiceAdmin
    | [<EndPoint "POST /documentsubmit">] DocumentSubmit
    | [<EndPoint "POST /imagesubmit">] ImageSubmit
    | [<EndPoint "/changelog">] Changelog
    | [<EndPoint "/";Wildcard>] NotFound of string

module EndPoints =
    //(endpoint,(minRole,maxRole))
    //If minRole is -2, the page is to be accessed logged out
    let PermissionList = Map [
        (EndPoint.Home,(-1,14));
        (EndPoint.Logout,(0,14));
        (EndPoint.Login,(-2,14));
        (EndPoint.Changelog,(0,14))
        (EndPoint.Registration,(-2,14));
        (EndPoint.PasswordChange,(0,14));
        (EndPoint.NameChange,(0,14));
        (EndPoint.Information,(0,14));
        (EndPoint.Delivery,(0,0));
        (EndPoint.Taxi,(1,6));
        (EndPoint.Towing,(7,14));
        (EndPoint.Documents,(0,14));
        (EndPoint.ImageUpload,(0,14));
        (EndPoint.ImageSubmit,(0,14));
        (EndPoint.DocumentSubmit,(0,14));
        (EndPoint.AdminHome,(11,14));
        (EndPoint.CallsAdmin,(11,14));
        (EndPoint.CarsAdmin,(11,14));
        (EndPoint.MembersAdmin,(12,14));
        (EndPoint.NameChangeAdmin,(12,14));
        (EndPoint.RegistrationAdmin,(12,14));
        (EndPoint.ServiceAdmin,(12,14));
    ]
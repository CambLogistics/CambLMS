namespace camblms

open WebSharper
open WebSharper.Sitelets

type EndPoint =
    | [<EndPoint "/">] Home
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
    | [<EndPoint "/logout">] Logout
    | [<EndPoint "/changelog">] Changelog

module EndPoints =
    //(endpoint,(minRole,maxRole))
    //If minRole is -2, the page is to be accessed logged out
    let PermissionList = Map [
        (EndPoint.Home,(-1,13));
        (EndPoint.Logout,(0,13));
        (EndPoint.Login,(-2,13));
        (EndPoint.Changelog,(0,13))
        (EndPoint.Registration,(-2,13));
        (EndPoint.PasswordChange,(0,13));
        (EndPoint.NameChange,(0,13));
        (EndPoint.Information,(0,13));
        (EndPoint.Delivery,(0,0));
        (EndPoint.Taxi,(1,6));
        (EndPoint.Towing,(7,13));
        (EndPoint.Documents,(0,13));
        (EndPoint.ImageUpload,(0,13));
        (EndPoint.ImageSubmit,(0,13));
        (EndPoint.DocumentSubmit,(0,13));
        (EndPoint.AdminHome,(11,13));
        (EndPoint.CallsAdmin,(11,13));
        (EndPoint.CarsAdmin,(11,13));
        (EndPoint.MembersAdmin,(12,13));
        (EndPoint.NameChangeAdmin,(12,13));
        (EndPoint.RegistrationAdmin,(12,13));
        (EndPoint.ServiceAdmin,(12,13));
    ]
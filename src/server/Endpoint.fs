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

module EndPoints =
    //(endpoint,minRole)
    //If minRole is -2, the page is to be accessed logged out
    let PermissionList = Map [
        (EndPoint.Home,-1);
        (EndPoint.Logout,0);
        (EndPoint.Login,-2);
        (EndPoint.Registration,-2);
        (EndPoint.PasswordChange,0);
        (EndPoint.NameChange,0);
        (EndPoint.Information,0);
        (EndPoint.Delivery,0);
        (EndPoint.Taxi,1);
        (EndPoint.Towing,7);
        (EndPoint.Documents,0);
        (EndPoint.ImageUpload,0);
        (EndPoint.ImageSubmit,0);
        (EndPoint.DocumentSubmit,0);
        (EndPoint.AdminHome,12);
        (EndPoint.CallsAdmin,12);
        (EndPoint.CarsAdmin,12);
        (EndPoint.MembersAdmin,12);
        (EndPoint.NameChangeAdmin,12);
        (EndPoint.RegistrationAdmin,12);
        (EndPoint.ServiceAdmin,12);
    ]
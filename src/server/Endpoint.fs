namespace camblms

open WebSharper

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/leave">] Logout
    | [<EndPoint "/login">] Login
    | [<EndPoint "/registration">] Registration
    | [<EndPoint "/passchange">] PasswordChange
    | [<EndPoint "/inactivity">] Inactivity
    | [<EndPoint "/inactivity_admin">] InactivityAdmin
    | [<EndPoint "/namechange">] NameChange
    | [<EndPoint "/forgotpass">] ForgotPass
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
    | [<EndPoint "/admin/docview">] DocAdmin
    | [<EndPoint "/admin/imgview">] ImgAdmin
    | [<EndPoint "POST /documentsubmit">] DocumentSubmit
    | [<EndPoint "POST /imagesubmit">] ImageSubmit
    | [<EndPoint "/changelog">] Changelog
    | [<EndPoint "/docs/";Wildcard>] DocServe of string
    | [<EndPoint "/img/";Wildcard>] ImgServe of string
    | [<EndPoint "/";Wildcard>] NotFound of string

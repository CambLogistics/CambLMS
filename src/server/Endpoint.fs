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
    | [<EndPoint "/admin/docview">] DocAdmin
    | [<EndPoint "/admin/imgview">] ImgAdmin
    | [<EndPoint "POST /documentsubmit">] DocumentSubmit
    | [<EndPoint "POST /imagesubmit">] ImageSubmit
    | [<EndPoint "/changelog">] Changelog
    | [<EndPoint "/";Wildcard>] NotFound of string
namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Templating

//Full templates with html tags
[<JavaScript>]
module SiteTemplates =
    let AdminStyle = "adminstyle.css"
    let NormalStyle = "normalstyle.css"
    let ChangeStyle = "changestyles.css"
    type MainTemplate = Templating.Template<"wwwroot/templates/main.html">
    type LoginTemplate = Templating.Template<"wwwroot/templates/loginwrapper.html">

//Parts not servable on their own
[<JavaScript>]
module SiteParts = 
    type LoginPage = Templating.Template<"wwwroot/templates/login.html">
    type RegistrationPage = Templating.Template<"wwwroot/templates/registration.html", serverLoad = ServerLoad.WhenChanged>
    type AdminHomeTemplate = Templating.Template<"wwwroot/templates/admin.html">
    type DeliveryTemplate = Templating.Template<"wwwroot/templates/delivery.html">
    type CarTemplate = Templating.Template<"wwwroot/templates/cars.html">
    type ChangelogTemplate = Templating.Template<"wwwroot/templates/changelog.html">
    type CallsTemplate = Templating.Template<"wwwroot/templates/calls.html">
    type InfoTemplate = Templating.Template<"wwwroot/templates/info.html">
    type DocumentsTemplate = Templating.Template<"wwwroot/templates/docs.html">
    type ImageUploadTemplate = Templating.Template<"wwwroot/templates/img.html">
    type NavTemplate = Templating.Template<"wwwroot/templates/navbar.html">
    type NameAdminTemplate = Templating.Template<"wwwroot/templates/name_admin.html">
    type NameChangeTemplate = Templating.Template<"wwwroot/templates/namechange.html">
    type PasswordChangeTemplate = Templating.Template<"wwwroot/templates/passchange.html"> 
    type RegistrationAdminTemplate = Templating.Template<"wwwroot/templates/reg_admin.html">
    type MemberListTemplate = Templating.Template<"wwwroot/templates/members.html">
    type TaxiTemplate = Templating.Template<"wwwroot/templates/taxi.html">
    type TowTemplate = Templating.Template<"wwwroot/templates/tow.html">

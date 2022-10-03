namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Templating

//Full templates with html tags
[<JavaScript>]
module SiteTemplates =
    let AdminStyle = "/adminstyle.css"
    let NormalStyle = "/normalstyle.css"
    let ChangeStyle = "/changestyles.css"
    type MainTemplate = Templating.Template<"templates/wrappers/main.html">
    type LoginTemplate = Templating.Template<"templates/wrappers/loginwrapper.html">

//Parts not serveable on their own
[<JavaScript>]
module SiteParts = 
    type LoginPage = Templating.Template<"templates/parts/login.html">
    type RegistrationPage = Templating.Template<"templates/parts/registration.html", serverLoad = ServerLoad.WhenChanged>
    type AdminHomeTemplate = Templating.Template<"templates/parts/admin.html">
    type CarTemplate = Templating.Template<"templates/parts/cars.html">
    type ChangelogTemplate = Templating.Template<"templates/parts/changelog.html">
    type ForgotPassTemplate = Templating.Template<"templates/parts/forgotpass.html">
    type ImageViewingTemplate = Templating.Template<"templates/parts/imgview.html">
    type DocViewingTemplate = Templating.Template<"templates/parts/docview.html">
    type CallsTemplate = Templating.Template<"templates/parts/calls.html">
    type InfoTemplate = Templating.Template<"templates/parts/info.html">
    type DocumentsTemplate = Templating.Template<"templates/parts/docs.html">
    type InactivityTemplate = Templating.Template<"templates/parts/inactivity.html">
    type InactivityAdminTemplate = Templating.Template<"templates/parts/inactivity_admin.html">
    type ImageUploadTemplate = Templating.Template<"templates/parts/img.html">
    type NameAdminTemplate = Templating.Template<"templates/parts/name_admin.html">
    type NameChangeTemplate = Templating.Template<"templates/parts/namechange.html">
    type PasswordChangeTemplate = Templating.Template<"templates/parts/passchange.html"> 
    type RegistrationAdminTemplate = Templating.Template<"templates/parts/reg_admin.html">
    type MemberListTemplate = Templating.Template<"templates/parts/members.html">
    type TaxiTemplate = Templating.Template<"templates/parts/taxi.html">
    type TowTemplate = Templating.Template<"templates/parts/tow.html">
    type ServiceTemplate = Templating.Template<"templates/parts/service.html">
    type NotFoundTemplate = Templating.Template<"templates/parts/notfound.html">


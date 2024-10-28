namespace camblms.templating

open WebSharper
open WebSharper.UI

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
    type AdminHomeTemplate = Templating.Template<"templates/parts/admin.html">
    type CarTemplate = Templating.Template<"templates/parts/cars.html">
    type ChangelogTemplate = Templating.Template<"templates/parts/changelog.html">
    type ForgotPassTemplate = Templating.Template<"templates/parts/forgotpass.html">
    type DocViewingTemplate = Templating.Template<"templates/parts/docsview.html">
    type InfoTemplate = Templating.Template<"templates/parts/info.html">
    type DocumentsTemplate = Templating.Template<"templates/parts/docs.html">
    type InactivityTemplate = Templating.Template<"templates/parts/inactivity.html">
    type SettingsTemplate = Templating.Template<"templates/parts/settings.html">
    type InactivityAdminTemplate = Templating.Template<"templates/parts/inactivity_admin.html">
    type ImageUploadTemplate = Templating.Template<"templates/parts/img.html">
    type NameChangeTemplate = Templating.Template<"templates/parts/namechange.html">
    type PasswordChangeTemplate = Templating.Template<"templates/parts/passchange.html">
    type RegistrationAdminTemplate = Templating.Template<"templates/parts/reg_admin.html">
    type MemberListTemplate = Templating.Template<"templates/parts/members.html">
    type TaxiTemplate = Templating.Template<"templates/parts/taxi.html">
    type TowTemplate = Templating.Template<"templates/parts/tow.html">
    type ServiceTemplate = Templating.Template<"templates/parts/service.html">
    type NotFoundTemplate = Templating.Template<"templates/parts/notfound.html">

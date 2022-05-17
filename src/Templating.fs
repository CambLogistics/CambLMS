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
    type MainTemplate = Templating.Template<"wwwroot/templates/main.html", ClientLoad.FromDocument, ServerLoad.WhenChanged>
    type LoginTemplate = Templating.Template<"wwwroot/templates/login.html", ClientLoad.FromDocument, ServerLoad.WhenChanged>
    type RegistrationTemplate = Templating.Template<"wwwroot/templates/registration.html", ClientLoad.FromDocument, ServerLoad.WhenChanged>

//Parts not servable on their own
[<JavaScript>]
module SiteParts = 
    type AdminHomeTemplate = Templating.Template<"wwwroot/templates/admin.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
    type DeliveryTemplate = Templating.Template<"wwwroot/templates/delivery.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
    type CarTemplate = Templating.Template<"wwwroot/templates/cars.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
    type ChangelogTemplate = Templating.Template<"wwwroot/templates/changelog.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
    type CallsTemplate = Templating.Template<"wwwroot/templates/calls.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
    type InfoTemplate = Templating.Template<"wwwroot/templates/info.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
    type DocumentsTemplate = Templating.Template<"wwwroot/templates/docs.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
    type ImageUploadTemplate = Templating.Template<"wwwroot/templates/img.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
    type NavTemplate = Templating.Template<"wwwroot/templates/navbar.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
    type NameAdminTemplate = Templating.Template<"wwwroot/templates/name_admin.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
    type NameChangeTemplate = Templating.Template<"wwwroot/templates/namechange.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
    type PasswordChangeTemplate = Templating.Template<"wwwroot/templates/passchange.html",ClientLoad.FromDocument,ServerLoad.WhenChanged> 
    type RegistrationAdminTemplate = Templating.Template<"wwwroot/templates/reg_admin.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
    type MemberListTemplate = Templating.Template<"wwwroot/templates/members.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
    type TaxiTemplate = Templating.Template<"wwwroot/templates/taxi.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
    type TowTemplate = Templating.Template<"wwwroot/templates/tow.html",ClientLoad.FromDocument,ServerLoad.WhenChanged>
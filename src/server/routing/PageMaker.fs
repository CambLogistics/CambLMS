namespace camblms.server.routing

open System.Reflection

open WebSharper.UI
open WebSharper.Sitelets

open camblms.dto
open camblms.templating
open camblms.server.service
open camblms.server.site
open camblms.sites

module PageMakers =
    let popupActiveString = "active"

    let getDarkModeStatus (ctx: Context<EndPoint>) =
        let cookieValue = ctx.Request.Cookies.Item "camblms_preferred_mode"

        if cookieValue.IsNone then "light-mode"
        else if cookieValue.Value.Equals "dark" then "dark-mode"
        else "light-mode"

    let versionInfo =
        let infVersionType = typedefof<AssemblyInformationalVersionAttribute>
        Assembly.GetCallingAssembly()
            .GetCustomAttributes(infVersionType,false)
        |> Seq.item 0 :?> AssemblyInformationalVersionAttribute

    let buildMainTemplate ctx ep (name: string) isAdmin =
        SiteTemplates
            .MainTemplate()
            .DarkMode(getDarkModeStatus ctx)
            .Navbar(Navbar.MakeNavbar ctx ep isAdmin)
            .FirstName(name)
            .VersionString(versionInfo.InformationalVersion)

    let RegistrationAdmin ctx ep (name: string) =
        (buildMainTemplate ctx ep name true)
            .ErrorBox(SiteTemplates.MainTemplate.ErrorMessageBox().Doc())
            .SuccessBox(SiteTemplates.MainTemplate.SuccessMessageBox().Doc())
            .Main(ClientServer.client (RegistrationAdminPage.RenderPage()))
            .Doc()

    let PasswordChange ctx ep (name: string) =
        (buildMainTemplate ctx ep name false)
            .ErrorBox(SiteTemplates.MainTemplate.ErrorMessageBox().Doc())
            .SuccessBox(SiteTemplates.MainTemplate.SuccessMessageBox().Doc())
            .Main(ClientServer.client (PasswordChangePage.RenderPage()))
            .Doc()

    let NameChange ctx ep (name: string) =
        (buildMainTemplate ctx ep name false)
            .ErrorBox(SiteTemplates.MainTemplate.ErrorMessageBox().Doc())
            .SuccessBox(SiteTemplates.MainTemplate.SuccessMessageBox().Doc())
            .Main(ClientServer.client (NameChangePage.RenderPage()))
            .Doc()

    let MembersAdmin ctx ep (user: Member) (name: string) =
        (buildMainTemplate ctx ep name true)
            .ErrorBox(SiteTemplates.MainTemplate.ErrorMessageBox().Doc())
            .SuccessBox(SiteTemplates.MainTemplate.SuccessMessageBox().Doc())
            .Main(ClientServer.client (MemberAdminPage.RenderPage user))
            .Doc()

    let Changelog ctx ep (name: string) =
        (buildMainTemplate ctx ep name false)
            .Main(SiteParts.ChangelogTemplate().Doc())
            .Doc()

    let CarsAdmin ctx ep (name: string) =
        (buildMainTemplate ctx ep name true)
            .ErrorBox(SiteTemplates.MainTemplate.ErrorMessageBox().Doc())
            .SuccessBox(SiteTemplates.MainTemplate.SuccessMessageBox().Doc())
            .Main(ClientServer.client (CarsAdmin.RenderPage()))
            .Doc()

    let Information ctx ep (name: string) =
        (buildMainTemplate ctx ep name false)
            .Main(ClientServer.client (Information.RenderPage()))
            .Doc()

    let Taxi ctx ep (name: string) =
        (buildMainTemplate ctx ep name false)
            .ErrorBox(SiteTemplates.MainTemplate.ErrorMessageBox().Doc())
            .SuccessBox(SiteTemplates.MainTemplate.SuccessMessageBox().Doc())
            .Main(ClientServer.client (TaxiPage.RenderPage()))
            .Doc()

    let Towing ctx ep (name: string) =
        (buildMainTemplate ctx ep name false)
            .ErrorBox(SiteTemplates.MainTemplate.ErrorMessageBox().Doc())
            .SuccessBox(SiteTemplates.MainTemplate.SuccessMessageBox().Doc())
            .Main(ClientServer.client (TowPage.RenderPage()))
            .Doc()

    let AdminHome ctx ep (name: string) =
        (buildMainTemplate ctx ep name true)
            .ErrorBox(SiteTemplates.MainTemplate.ErrorMessageBox().Doc())
            .SuccessBox(SiteTemplates.MainTemplate.SuccessMessageBox().Doc())
            .Main(ClientServer.client (AdminHome.RenderPage()))
            .Doc()

    let ServiceAdmin ctx ep (name: string) =
        (buildMainTemplate ctx ep name true)
            .ErrorBox(SiteTemplates.MainTemplate.ErrorMessageBox().Doc())
            .SuccessBox(SiteTemplates.MainTemplate.SuccessMessageBox().Doc())
            .Main(ClientServer.client (ServiceFeeAdmin.RenderPage()))
            .Doc()

    let LoginPage ctx =
        SiteTemplates
            .LoginTemplate()
            .SuccessBox(SiteTemplates.LoginTemplate.SuccessMessageBox().Doc())
            .ErrorBox(SiteTemplates.LoginTemplate.ErrorMessageBox().Doc())
            .Main(ClientServer.client (LoginPage.RenderPage()))
            .Doc()

    let InactivityPage ctx ep (name: string) =
        (buildMainTemplate ctx ep name false)
            .ErrorBox(SiteTemplates.MainTemplate.ErrorMessageBox().Doc())
            .SuccessBox(SiteTemplates.MainTemplate.SuccessMessageBox().Doc())
            .Main(ClientServer.client (InactivityPage.RenderPage()))
            .Doc()

    let InactivityAdmin ctx ep (name: string) =
        (buildMainTemplate ctx ep name true)
            .ErrorBox(SiteTemplates.MainTemplate.ErrorMessageBox().Doc())
            .SuccessBox(SiteTemplates.MainTemplate.SuccessMessageBox().Doc())
            .Main(ClientServer.client (InactivityAdmin.RenderPage()))
            .Doc()

    let DocAdmin ctx ep (name: string) =
        (buildMainTemplate ctx ep name true)
            .ErrorBox(SiteTemplates.MainTemplate.ErrorMessageBox().Doc())
            .SuccessBox(SiteTemplates.MainTemplate.SuccessMessageBox().Doc())
            .Main(ClientServer.client (DocAdmin.RenderPage()))
            .Doc()

    let ForgotPass ctx =
        SiteTemplates
            .LoginTemplate()
            .SuccessBox(SiteTemplates.LoginTemplate.SuccessMessageBox().Doc())
            .ErrorBox(SiteTemplates.LoginTemplate.ErrorMessageBox().Doc())
            .Main(ClientServer.client (ForgotPassPage.RenderPage()))
            .Doc()

    let Logout (ctx: Context<EndPoint>) =
        ctx.Request.Cookies.Item "clms_sid" |> Option.get |> UserService.logoutUser

        SiteTemplates
            .MainTemplate()
            .Navbar()
            .Main(ClientServer.client (LogoutClient.Logout()))
            .Doc()

    let SettingsPage (ctx: Context<EndPoint>) ep (name: string) (user: Member) =
        (buildMainTemplate ctx ep name false)
            .Main(ClientServer.client (SettingsPage.RenderPage()))
            .Doc()

    let DocumentPage (ctx: Context<EndPoint>) ep (name: string) (user: Member) =
        (buildMainTemplate ctx ep name false)
            .ErrorBox(
                match ctx.Request.Get.Item "success" with
                | Some s ->
                    if not (s = "true") then
                        SiteTemplates.MainTemplate
                            .ErrorMessageBox()
                            .Active(popupActiveString)
                            .StatusMessage("Hiba a dokumentumfeltöltés közben! Keresd a (műszaki) igazgatót!")
                            .Doc()
                    else
                        SiteTemplates.MainTemplate.ErrorMessageBox().Doc()
                | None -> SiteTemplates.MainTemplate.ErrorMessageBox().Doc()
            )
            .SuccessBox(
                match ctx.Request.Get.Item "success" with
                | Some s ->
                    if s = "true" then
                        SiteTemplates.MainTemplate
                            .SuccessMessageBox()
                            .StatusMessage("Sikeres iratfeltöltés!")
                            .Active(popupActiveString)
                            .Doc()
                    else
                        SiteTemplates.MainTemplate.SuccessMessageBox().Doc()
                | None -> SiteTemplates.MainTemplate.SuccessMessageBox().Doc()
            )
            .Main(
                SiteParts
                    .DocumentsTemplate()
                    .PersonalID(
                        if System.IO.File.Exists("docs/" + string user.AccountID + "_personal.png") then
                            ("/docs/" + string user.AccountID + "_personal.png")
                        else
                            ""
                    )
                    .DriversLicense(
                        if System.IO.File.Exists("docs/" + string user.AccountID + "_license.png") then
                            ("/docs/" + string user.AccountID + "_license.png")
                        else
                            ""
                    )
                    .Doc()
            )
            .Doc()

    let ImageUpload ctx ep (name: string) =
        (buildMainTemplate ctx ep name false)
            .ErrorBox(
                match ctx.Request.Get.Item "success" with
                | Some s ->
                    if (not (s = "true")) && (not (s = "inactivity")) then
                        SiteTemplates.MainTemplate
                            .ErrorMessageBox()
                            .Active(popupActiveString)
                            .StatusMessage("Hiba a szervizkép feltöltés közben! Keresd a (műszaki) igazgatót!")
                            .Doc()
                    else if s = "inactivity" then
                        SiteTemplates.MainTemplate
                            .ErrorMessageBox()
                            .Active(popupActiveString)
                            .StatusMessage("Szabadság alatt nem tölthetsz fel szervizképet!")
                            .Doc()
                    else
                        SiteTemplates.MainTemplate.ErrorMessageBox().Doc()
                | None -> SiteTemplates.MainTemplate.ErrorMessageBox().Doc()
            )
            .SuccessBox(
                match ctx.Request.Get.Item "success" with
                | Some s ->
                    if s = "true" then
                        SiteTemplates.MainTemplate
                            .SuccessMessageBox()
                            .Active(popupActiveString)
                            .StatusMessage("Sikeres iratfeltöltés!")
                            .Doc()
                    else
                        SiteTemplates.MainTemplate.SuccessMessageBox().Doc()
                | None -> SiteTemplates.MainTemplate.SuccessMessageBox().Doc()
            )
            .Main(SiteParts.ImageUploadTemplate().Doc())
            .Doc()

    let NotFound ctx =
        SiteTemplates
            .MainTemplate()
            .DarkMode(getDarkModeStatus ctx)
            .FirstName("látogató")
            .Navbar()
            .Main(
                SiteParts
                    .NotFoundTemplate()
                    .ErrorMessage("A keresett oldal nem található!")
                    .Doc()
            )
            .Doc()

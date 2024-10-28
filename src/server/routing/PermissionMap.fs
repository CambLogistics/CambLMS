namespace camblms.server.routing

open camblms.dto

module PermissionMap =
    let RequiredPermissions =
        Map
            [ (EndPoint.Home, Permissions.Nothing)
              (EndPoint.Logout, Permissions.Nothing)
              (EndPoint.Login, Permissions.Nothing)
              (EndPoint.ForgotPass, Permissions.Nothing)
              (EndPoint.Changelog, Permissions.Nothing)
              (EndPoint.Inactivity, Permissions.Nothing)
              (EndPoint.Settings, Permissions.Nothing)
              (EndPoint.PasswordChange, Permissions.Nothing)
              (EndPoint.NameChange, Permissions.Nothing)
              (EndPoint.Information, Permissions.Nothing)
              (EndPoint.Taxi, Permissions.TaxiCall)
              (EndPoint.Towing, Permissions.TowCall)
              (EndPoint.Settings, Permissions.Nothing)
              (EndPoint.Documents, Permissions.Nothing)
              (EndPoint.ImageUpload, Permissions.Nothing)
              (EndPoint.ImageSubmit, Permissions.Nothing)
              (EndPoint.DocumentSubmit, Permissions.Nothing)
              (EndPoint.AdminHome, Permissions.Admin)
              (EndPoint.CarsAdmin, Permissions.ViewCars)
              (EndPoint.InactivityAdmin, Permissions.InactivityAdmin)
              (EndPoint.DocAdmin, Permissions.DocAdmin)
              (EndPoint.MembersAdmin, Permissions.MemberAdmin)
              (EndPoint.RegistrationAdmin, Permissions.MemberAdmin)
              (EndPoint.ServiceAdmin, Permissions.ServiceFeeAdmin) ]

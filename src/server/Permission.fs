namespace camblms

open WebSharper

[<JavaScript>]
[<System.Flags>]
type Permissions =
    |Nothing = 2047u
    |Admin = 2040u
    |DeliveryCall = 1u
    |TowCall = 2u
    |TaxiCall = 4u
    |ViewCars = 8u
    |CarAdmin = 16u
    |ViewCallCount = 32u
    |CloseWeek = 64u
    |ServiceFeeAdmin = 128u
    |TriggerDoublePrice = 256u
    |DocAdmin = 512u
    |MemberAdmin = 1024u

module Permission = 
    let RequiredPermissions = Map [
        (EndPoint.Home,Permissions.Nothing);
        (EndPoint.Logout,Permissions.Nothing);
        (EndPoint.Login,Permissions.Nothing);
        (EndPoint.Changelog,Permissions.Nothing)
        (EndPoint.Registration,Permissions.Nothing);
        (EndPoint.PasswordChange,Permissions.Nothing);
        (EndPoint.NameChange,Permissions.Nothing);
        (EndPoint.Information,Permissions.Nothing);
        (EndPoint.Delivery,Permissions.DeliveryCall);
        (EndPoint.Taxi,Permissions.TaxiCall);
        (EndPoint.Towing,Permissions.TowCall);
        (EndPoint.Documents,Permissions.Nothing);
        (EndPoint.ImageUpload,Permissions.Nothing);
        (EndPoint.ImageSubmit,Permissions.Nothing);
        (EndPoint.DocumentSubmit,Permissions.Nothing);
        (EndPoint.AdminHome,Permissions.Admin);
        (EndPoint.CallsAdmin,Permissions.ViewCallCount);
        (EndPoint.CarsAdmin,Permissions.ViewCars);
        (EndPoint.DocAdmin,Permissions.DocAdmin);
        (EndPoint.ImgAdmin,Permissions.ServiceFeeAdmin);
        (EndPoint.MembersAdmin,Permissions.MemberAdmin);
        (EndPoint.NameChangeAdmin,Permissions.MemberAdmin);
        (EndPoint.RegistrationAdmin,Permissions.MemberAdmin);
        (EndPoint.ServiceAdmin,Permissions.ServiceFeeAdmin);
    ]
    let getUserPermissions user =
        try
            let db = Database.SqlConnection.GetDataContext (Database.getConnectionString())
            let rolePermissions = 
                (query{
                    for p in db.Camblogistics.permissions do
                    where (p.RoleId = user.Role)
                    exactlyOne
                }).Permissions
            let userOverride = 
                let upo = 
                    (query{
                        for po in db.Camblogistics.permissionoverrides do
                        where(po.UserId = user.Id)
                        select po.Permissions
                    })
                if Seq.isEmpty upo then 0u
                else Seq.item 0 upo
            rolePermissions ||| userOverride
        with
            _ -> 0u
    let checkPermission sid (perm:Permissions) =
        match User.getUserFromSID sid with
            |None -> false
            |Some u -> ((getUserPermissions u &&& (LanguagePrimitives.EnumToValue perm)) > 0u)
    [<Rpc>]
    let doCheckPermission sid perm =
        async{
            return checkPermission sid perm
        } 
            


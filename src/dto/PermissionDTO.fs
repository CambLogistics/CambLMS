namespace camblms.dto

open WebSharper

[<JavaScript>]
[<System.Flags>]
type Permissions =
    | Nothing = 4095u
    | Admin = 4088u
    | DeliveryCall = 1u //Left here for backwards-compatibility reasons
    | TowCall = 2u
    | TaxiCall = 4u
    | ViewCars = 8u
    | CarAdmin = 16u
    | ViewCallCount = 32u
    | CloseWeek = 64u
    | ServiceFeeAdmin = 128u
    | TriggerDoublePrice = 256u
    | DocAdmin = 512u
    | MemberAdmin = 1024u
    | InactivityAdmin = 2048u

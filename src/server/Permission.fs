namespace camblms

open WebSharper

[<JavaScript>]
type Permissions =
    |DeliveryCall = 1
    |TowCall = 2
    |TaxiCall = 4
    |ViewCars = 8
    |CarAdmin = 16
    |ViewCallCount = 32
    |CloseWeek = 64
    |TriggerDoublePrice = 128
    |MemberAdmin = 256
    |DocAdmin = 512
    |ServiceFeeAdmin = 1024
    |Everything = 2048

            


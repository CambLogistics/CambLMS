namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module DeliveryPage =
    let TypeList = ListModel.FromSeq [{ID= -1;Name="";Price=0}]
    let selectedType = Var.Create {ID= -1;Name="";Price=0}
    let updateTypeList() =
        async{
            let! list = Delivery.doGetTypeList()
            TypeList.Set list
        } |> Async.Start
    let RenderPage() =
        updateTypeList()
        SiteParts.DeliveryTemplate()
            .PriceDisplay(
                selectedType.View.Map(
                    fun t ->
                        if t.ID = -1 then 
                            JavaScript.JS.Document.GetElementById("Submit").SetAttribute("disabled","true")
                            "Jelenleg nincs kiválasztva szolgáltatás."
                        else
                            JavaScript.JS.Document.GetElementById("Submit").RemoveAttribute("disabled") 
                            "A szolgáltatás ára: " + string t.Price
                )
            )
            .DeliveryTypeList(
                TypeList.View |> Doc.BindSeqCached (
                    fun t ->
                        SiteParts.DeliveryTemplate.DeliveryTypeItem()
                            .Name(t.Name)
                            .TypeID(string t.ID)
                            .Doc()
                )
            )
            .DeliveryType(
                selectedType.Lens (fun t -> string t.ID) (fun t newID -> query{
                    for ty in TypeList.Value do
                    where(ty.ID = int newID)
                    exactlyOne
                    })
            )
            .Submit(
                fun e ->
                    async{
                        let sessionID = JavaScript.Cookies.Get("clms_sid").Value
                        let! result = Delivery.submitCall sessionID selectedType.Value.ID
                        match result with
                            |CallResult.Success -> 
                                Feedback.giveFeedback false "Sikeres művelet!"
                                selectedType.Set {ID= -1; Name="";Price=0}
                                JavaScript.JS.Window.Location.Replace "/"
                            |CallResult.InvalidSession -> Feedback.giveFeedback true "Érvénytelen munkamenet. Lépj ki és be újra!"
                            |CallResult.DatabaseError -> Feedback.giveFeedback true "Adatbázishiba. Keresd a (műszaki) igazgatót!"
                    } |> Async.Start
            )
            .Reset(
                fun _ -> 
                    selectedType.Set {ID= -1; Name="";Price=0}
                    JavaScript.JS.Window.Location.Replace "/"
                    )
            .Doc()
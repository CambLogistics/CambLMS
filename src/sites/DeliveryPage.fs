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
            .Reset(fun _ -> selectedType.Set {ID= -1; Name="";Price=0})
            .Accept(
                fun _ ->
                    async{
                        let! result = Delivery.submitCall (JavaScript.Cookies.Get "clms_sid").Value selectedType.Value.ID
                        match result with
                            |CallResult.Success -> 
                                Feedback.giveFeedback false "Sikeres művelet!"
                                selectedType.Set {ID= -1; Name="";Price=0}
                            |CallResult.InvalidSession -> Feedback.giveFeedback true "Érvénytelen munkamenet. Lépj ki és be újra!"
                            |CallResult.DatabaseError -> Feedback.giveFeedback true "Adatbázishiba. Keresd a (műszaki) igazgatót!"
                    } |> Async.Start
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
            .SetSelected(
                fun e ->
                    query{
                        for t in TypeList.Value do
                        where(t.ID = int e.Vars.DeliveryType.Value)
                        exactlyOne
                    } |> selectedType.Set
            )
            .Doc()
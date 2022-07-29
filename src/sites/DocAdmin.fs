namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module DocAdmin =
    let userList = ListModel.Create (fun (u:Member) -> u) []
    let selectedMember = Var.Create -1
    let updateUserList() =
        async{
            let! list = Documents.doGetUsersWithDocuments (JavaScript.Cookies.Get("clms_sid").Value)
            userList.Set list
            if selectedMember.Value = -1 then selectedMember.Set <| (Seq.head userList.Value).Id
        } |> Async.Start
    let RenderPage() =
        updateUserList()
        SiteParts.DocViewingTemplate()
            .Show(
                fun _ ->
                    JavaScript.JS.Document.GetElementById("docs").RemoveAttribute("style")
            )
            .Member(selectedMember.Lens (fun m -> string m) (fun m mi -> int mi))
            .MemberList(
                Doc.BindSeqCached(
                    fun (u:Member) ->
                        SiteParts.DocViewingTemplate.MemberItem()
                            .MemberID(string u.Id)
                            .MemberName(u.Name)
                            .Doc()

                ) userList.View
            )
            .PersonalID(
               View.Map (
                fun m ->
                  let accID = 
                    (query{
                        for u in userList do
                            where(u.Id = m)
                            exactlyOne
                    }).AccountID
                  "/docs/" + string accID + "_personal.png"
               ) selectedMember.View
            )
            .DriversLicense(
               View.Map (
                fun m ->
                  let accID = 
                    (query{
                        for u in userList do
                            where(u.Id = m)
                            exactlyOne
                    }).AccountID
                  "/docs/" + string accID + "_license.png"
               ) selectedMember.View
            )
            .Doc()

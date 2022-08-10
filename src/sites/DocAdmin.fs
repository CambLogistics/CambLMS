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
                  let userById = userList |> Seq.filter (fun u -> u.Id = m)
                  if (Seq.length userById > 0) then "/docs/" + (string (Seq.item 0 userById).AccountID) + "_personal.png" else ""
               ) selectedMember.View
            )
            .DriversLicense(
               View.Map (
                fun m ->
                  let userById = userList |> Seq.filter (fun u -> u.Id = m)
                  if Seq.length userById > 0 then "/docs/" + (string (Seq.item 0 userById).AccountID) + "_license.png" else ""
               ) selectedMember.View
            )
            .Doc()

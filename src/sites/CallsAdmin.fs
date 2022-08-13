namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module CallsAdmin =
    let userList = ListModel.Create (fun (u:UserWithCalls) -> u) []
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
    let rankList = Var.Create [{Level = 0;Name="Beszállító"}]
    let mutable canClose = false
    let updateUserList() =
        async{
            let! list = Calls.getUserListWithCalls sessionID CallDuration.Weekly
            let! cc = Permission.doCheckPermission sessionID Permissions.CloseWeek
            canClose <- cc
            userList.Set list
            if cc then JavaScript.JS.Document.GetElementById("closeweek").RemoveAttribute("style")
        } |> Async.Start
    let updateRankList() =
        async{
            let! list = UserCallable.doGetRankList()
            rankList.Set list
        } |> Async.Start
    let RenderPage() =
        updateRankList()
        updateUserList()
        SiteParts.CallsTemplate()
            .MemberList(
                userList.View |> Doc.BindSeqCached (
                    fun u ->
                        SiteParts.CallsTemplate.Member()
                            .Name(u.User.Name)
                            .CallNum(u.Calls |> List.length |> string)
                            .Rank(
                                (query{
                                    for rank in rankList.Value do
                                    where(rank.Level = u.User.Role)
                                    exactlyOne
                                }).Name
                            )
                            .Doc()
                )
            )
            .CloseWeek(
                fun _ ->
                    if not canClose then ()
                    else
                        ActionDispatcher.RunAction Calls.rotateWeek sessionID (Some updateUserList)
            )
            .Doc()

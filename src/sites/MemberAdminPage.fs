namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module MemberAdminPage =
    let UserList = ListModel.Create (fun (u:Member) -> u) []
    let RankList = ListModel.FromSeq [ { Level = -1; Name = "???" } ]
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value
    let updateRankList() =
        async {
            let! list = User.getRankList()
            match list with
                |Ok l -> RankList.Set l
                |Error e ->
                    Feedback.giveFeedback true <| "Hiba a rangok lekérésekor: " + e
        } |> Async.Start

    let updateUserList() =
        async {
            let! list = UserOperations.getUserList sessionID false false
            match list with
                |Ok l ->
                    UserList.Set l
                |Error e ->
                    Feedback.giveFeedback true <| "Hiba a felhasználók lekérésekor: " + e
        }
        |> Async.Start

    let RenderPage currentUser =
        updateRankList()
        updateUserList()
        SiteParts
            .MemberListTemplate()
            .UserList(
                UserList.View
                |> Doc.BindSeqCached (fun u ->
                    SiteParts
                        .MemberListTemplate
                        .UserListItem()
                        .ID(string u.Id)
                        .Name(u.Name)
                        .AccID(string u.AccountID)
                        .RoleSelectHole(
                            if u.Role >= currentUser.Role
                               || currentUser.Id = u.Id then
                                SiteParts
                                    .MemberListTemplate
                                    .RoleSelectorDisabled()
                                    .RankList(
                                        RankList.View
                                        |> Doc.BindSeqCached (fun r ->
                                            SiteParts
                                                .MemberListTemplate
                                                .RankListItem()
                                                .Level(string r.Level)
                                                .Name(r.Name)
                                                .Doc())
                                    )
                                    .Rank(string u.Role)
                                    .Doc()
                            else
                                SiteParts
                                    .MemberListTemplate
                                    .RoleSelectorEnabled()
                                    .RankList(
                                        RankList.View
                                        |> Doc.BindSeqCached (fun r ->
                                            if (r.Level >= currentUser.Role) then Doc.Empty
                                            else
                                            SiteParts
                                                .MemberListTemplate
                                                .RankListItem()
                                                .Level(string r.Level)
                                                .Name(r.Name)
                                                .Doc())
                                    )
                                    .Rank(string u.Role)
                                    .ChangeRank(
                                        fun e ->
                                            ActionDispatcher.RunAction UserOperations.changeUserRank (sessionID,u.Id,(int e.Vars.Rank.Value)) (Some updateUserList)
                                    )
                                    .Doc()
                        )
                        .DeleteButtonHole(
                            if currentUser.Id = u.Id
                               || currentUser.Role <= u.Role then
                                Doc.Empty
                            else
                                SiteParts
                                    .MemberListTemplate
                                    .DeleteButton()
                                    .DeleteUser(fun e ->
                                            ActionDispatcher.RunAction UserOperations.deleteUser (sessionID,u.Id) (Some updateUserList)
                                    )
                                    .Doc()
                        )
                        .Doc())
            ).Doc()

namespace camblms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module MemberAdminPage =
    let UserList =
        ListModel.FromSeq [ { Id = -1
                              Role = 0
                              AccountID = 0
                              Name = "Whoever"
                              Email = "what@what.no" } ]

    let RankList = ListModel.FromSeq [ { Level = -1; Name = "???" } ]
    let sessionID = JavaScript.Cookies.Get("clms_sid").Value

    let updateRankList() =
        async {
            let! list = UserCallable.doGetRankList()
            RankList.Set list
        } |> Async.Start

    let updateUserList() =
        async {
            let! list = UserCallable.doGetUserList sessionID false
            UserList.Set list
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
                                            async {
                                                let! result =
                                                    UserCallable.doChangeUserRank sessionID u.Id (int e.Vars.Rank.Value)
                                                updateUserList result
                                            } |> Async.Start
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
                                        async {
                                            let! finished = UserCallable.doDeleteUser sessionID u.Id
                                            updateUserList finished
                                        }
                                        |> Async.Start)
                                    .Doc()
                        )
                        .Doc())
            ).Doc()

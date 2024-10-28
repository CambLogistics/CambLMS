namespace camblms.sites

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

open camblms.dto
open camblms.templating
open camblms.server.controller

[<JavaScript>]
module MemberAdminPage =
    let UserList = ListModel.Create (fun (u: Member) -> u) []
    let RankList = ListModel.FromSeq [ { Level = -1; Name = "???" } ]
    let BlackList = ListModel.Create (fun (bli: BlacklistItem) -> bli) []

    let selectedBlackListItem =
        Var.Create
            { AccountID = 0
              UserName = ""
              Role = 1
              Reason = ""
              Comeback = false }

    let sessionID = JavaScript.Cookies.Get("clms_sid").Value

    let updateRankList () =
        async {
            let! list = UserController.getRankList ()

            match list with
            | Ok l -> RankList.Set l
            | Error e -> Feedback.giveFeedback true <| "Hiba a rangok lekérésekor: " + e
        }
        |> Async.Start

    let updateUserList () =
        async {
            let! list = UserController.getUserList sessionID false false

            match list with
            | Ok l -> UserList.Set l
            | Error e -> Feedback.giveFeedback true <| "Hiba a felhasználók lekérésekor: " + e
        }
        |> Async.Start

    let updateBlacklist () =
        async {
            let! list = BlacklistController.getBlackListItems sessionID

            match list with
            | Ok l -> BlackList.Set l
            | Error e -> Feedback.giveFeedback true <| "Hiba a feketelista lekérdezésekor: " + e
        }
        |> Async.Start

    let RenderPage (currentUser: Member) =
        updateRankList ()
        updateUserList ()
        updateBlacklist ()

        SiteParts
            .MemberListTemplate()
            .ActiveUserList(
                UserList.View
                |> Doc.BindSeqCached(fun u ->
                    SiteParts.MemberListTemplate
                        .ActiveUserListItem()
                        .ID(string u.Id)
                        .Name(u.Name)
                        .AccID(string u.AccountID)
                        .RoleSelectHole(
                            if u.Role >= currentUser.Role || currentUser.Id = u.Id then
                                SiteParts.MemberListTemplate
                                    .RoleSelectorDisabled()
                                    .RankList(
                                        RankList.View
                                        |> Doc.BindSeqCached(fun r ->
                                            SiteParts.MemberListTemplate
                                                .RankListItem()
                                                .Level(string r.Level)
                                                .Name(r.Name)
                                                .Doc())
                                    )
                                    .Rank(string u.Role)
                                    .Doc()
                            else
                                SiteParts.MemberListTemplate
                                    .RoleSelectorEnabled()
                                    .RankList(
                                        RankList.View
                                        |> Doc.BindSeqCached(fun r ->
                                            if (r.Level >= currentUser.Role.Level) then
                                                Doc.Empty
                                            else
                                                SiteParts.MemberListTemplate
                                                    .RankListItem()
                                                    .Level(string r.Level)
                                                    .Name(r.Name)
                                                    .Doc())
                                    )
                                    .Rank(string u.Role)
                                    .ChangeRank(fun e ->
                                        ActionDispatcher.RunAction
                                            UserController.changeUserRank
                                            (sessionID, u.Id, (int e.Vars.Rank.Value))
                                            (Some updateUserList))
                                    .Doc()
                        )
                        .DeleteButtonHole(
                            if currentUser.Id = u.Id || currentUser.Role <= u.Role then
                                Doc.Empty
                            else
                                SiteParts.MemberListTemplate
                                    .DeleteButton()
                                    .DeleteUser(fun e ->
                                        selectedBlackListItem.Set
                                            { AccountID = u.AccountID
                                              Role = u.Role.Level
                                              UserName = u.Name
                                              Reason = ""
                                              Comeback = false }

                                        ActionDispatcher.RunAction
                                            UserController.deleteUser
                                            (sessionID, u.Id)
                                            (Some updateUserList))
                                    .Doc()
                        )
                        .Doc())
            )
            .BLAccID(selectedBlackListItem.Lens (fun i -> string i.AccountID) (fun i s -> { i with AccountID = int s }))
            .BLRankList(
                RankList.View
                |> Doc.BindSeqCached(fun r ->
                    if (r.Level >= currentUser.Role.Level) then
                        Doc.Empty
                    else
                        SiteParts.MemberListTemplate
                            .BLRankListItem()
                            .Level(string r.Level)
                            .Name(r.Name)
                            .Doc())
            )
            .BLRank(selectedBlackListItem.Lens (fun i -> string i.Role) (fun i s -> { i with Role = int s }))
            .BLName(selectedBlackListItem.LensAuto(fun i -> i.UserName))
            .BLComeBack(selectedBlackListItem.LensAuto(fun i -> i.Comeback))
            .BLReason(selectedBlackListItem.LensAuto(fun i -> i.Reason))
            .RemovedUserList(
                BlackList.View
                |> Doc.BindSeqCached(fun bli ->
                    SiteParts.MemberListTemplate
                        .RemovedUserListItem()
                        .AccID(string bli.AccountID)
                        .ComeBack(if bli.Comeback then "igen" else "nem")
                        .Name(bli.UserName)
                        .Reason(bli.Reason)
                        .Rank(
                            (query {
                                for r in RankList do
                                    where (r.Level = bli.Role)
                                    exactlyOne
                            })
                                .Name
                        )
                        .Edit(fun _ -> selectedBlackListItem.Set bli)
                        .Remove(fun _ ->
                            ActionDispatcher.RunAction
                                BlacklistController.deleteItem
                                (sessionID, bli)
                                (Some updateBlacklist))
                        .Doc())
            )
            .BLConfirm(fun _ ->
                ActionDispatcher.RunAction
                    BlacklistController.setBlackListItem
                    (sessionID, selectedBlackListItem.Value)
                    (Some(fun () ->
                        updateBlacklist ()

                        selectedBlackListItem.Set
                            { AccountID = 0
                              UserName = ""
                              Role = 1
                              Reason = ""
                              Comeback = false })))
            .Doc()

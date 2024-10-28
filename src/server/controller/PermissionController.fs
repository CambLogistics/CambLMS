namespace camblms.server.controller

open WebSharper

open camblms.server.service

module PermissionController =
    [<Rpc>]
    let doCheckPermission sid perm =
        async { return PermissionService.checkPermission sid perm }

namespace camblms

open WebSharper
open WebSharper.UI

module ForgotPassPage =
        let RenderPage() =
                SiteParts.ForgotPassTemplate()
                        .Doc()

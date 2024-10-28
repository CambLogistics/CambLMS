namespace camblms.sites

open WebSharper

[<JavaScript>]
module Feedback =
    let giveFeedback isError message =
        let feedback =
            JavaScript.JS.Document.GetElementById(if isError then "errorMessage" else "successMessage")

        feedback.InnerHTML <- message

        (JavaScript.JS.Document.GetElementsByClassName(
            if isError then
                "alert-error-popup"
            else
                "alert-success-popup"
        )[0])
            .ChildNodes[0]
            .ParentElement.SetAttribute("id", "active")

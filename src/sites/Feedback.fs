namespace camblms

open WebSharper

[<JavaScript>]
module Feedback =
    let giveFeedback isError message=
        (JavaScript.JS.Document.GetElementById (if isError then "success" else "error")).SetAttribute("style","display:none;")
        let feedback = JavaScript.JS.Document.GetElementById (if isError then "error" else "success")
        feedback.InnerHTML <- message
        feedback.SetAttribute("style","display:block;")
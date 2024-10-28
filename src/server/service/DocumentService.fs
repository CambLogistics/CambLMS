namespace camblms.server.service

open FSharp.Data.Sql

open camblms.server
open camblms.server.database
open camblms.dto
open System.IO

module DocumentService =
    let getUsersWithValidDocuments () =
        let users = UserService.getAllUsers ()

        users
        |> Seq.toList
        |> List.filter (fun u ->
            File.Exists(@"docs/" + string u.AccountID + "_personal.png")
            && File.Exists(@"docs/" + string u.AccountID + "_license.png"))
        |> Ok

    let deleteDocument filename =
        let db = Database.getDataContext ()

        if File.Exists(@"img/" + filename) then
            File.Delete(@"img/" + filename)

        (query {
            for f in db.Camblogistics.images do
                where (f.Name = filename)
                exactlyOne
        })
            .Delete()

        db.SubmitUpdates()

    let uploadDocuments
        (personalStream: Stream, personalFilename: string)
        (licenseStream: Stream, licenseFilename: string)
        (user: Member)
        =
        if Directory.Exists "docs" |> not then
            Directory.CreateDirectory "docs" |> ignore

        let personal =
            File.Create(
                @"docs/"
                + string user.AccountID
                + "_personal"
                + Path.GetExtension personalFilename
            )

        let license =
            File.Create(
                @"docs/"
                + string user.AccountID
                + "_license"
                + Path.GetExtension licenseFilename
            )

        personalStream.CopyTo(personal)
        licenseStream.CopyTo(license)

    let getImageList () =
        let db = Database.getDataContext ()

        query {
            for i in db.Camblogistics.images do
                select (i.Name, i.Userid, i.UploadDate)
        }
        |> Seq.sortByDescending (fun (_, _, date) -> date)

    let uploadServiceImage (inputStream: Stream) (originalFileName: string) (user: Member) =
        let db = Database.getDataContext ()
        let fileName = RandomString.getRandomString 32 + Path.GetExtension originalFileName

        if Directory.Exists "img" |> not then
            Directory.CreateDirectory "img" |> ignore

        let fileStream = File.Create(@"img/" + fileName)
        inputStream.CopyTo(fileStream)
        let newFileEntry = db.Camblogistics.images.Create()
        newFileEntry.Userid <- user.Id
        newFileEntry.Name <- fileName
        newFileEntry.UploadDate <- System.DateTime.Now
        db.SubmitUpdates()

namespace camblms

open FSharp.Data.Sql

module Database =
    type SqlConnection = SqlDataProvider<Common.DatabaseProviderTypes.MYSQL,  "Server=localhost;Database=camblogistics;Uid=camblms;Pwd=V3l3tlen_J3lsz0;",UseOptionTypes = true,CaseSensitivityChange = Common.CaseSensitivityChange.TOLOWER>
    let getConnectionString() =
        if System.IO.File.Exists "db.conf" then
            use fs = new System.IO.FileStream("db.conf",System.IO.FileMode.Open)
            use ss = new System.IO.StreamReader(fs)
            ss.ReadToEnd()
        else
            "Server=localhost;Database=camblogistics;Uid=camblms;Pwd=V3l3tlen_J3lsz0;"
    let getDataContext() =
        SqlConnection.GetDataContext (getConnectionString())
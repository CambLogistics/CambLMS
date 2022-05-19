namespace camblms

open FSharp.Data.Sql

module Database =
    type SqlConnection = SqlDataProvider<Common.DatabaseProviderTypes.MYSQL,  "Server=localhost;Database=camblogistics;Uid=camblms;Pwd=V3l3tlen_J3lsz0;",UseOptionTypes = true>
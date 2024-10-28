namespace camblms.server.database

open FSharp.Data.Sql

open camblms.server.config

module Database =
    type SqlConnection =
        SqlDataProvider<
            Common.DatabaseProviderTypes.MYSQL,
            "Server=localhost;Database=camblogistics;Uid=camblms;Pwd=V3l3tlen_J3lsz0;",
            UseOptionTypes=Common.NullableColumnType.OPTION,
            CaseSensitivityChange=Common.CaseSensitivityChange.TOLOWER
         >

    let getConnectionString () =
        let config = Config.readDatabase ()

        sprintf
            "Server=%s;Port=%i;Database=%s;Uid=%s;Pwd=%s"
            config.Host
            config.Port
            config.DatabaseName
            config.Username
            config.Password

    let dataContext = SqlConnection.GetDataContext(getConnectionString ())
    let getDataContext () = dataContext

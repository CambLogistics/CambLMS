namespace camblms.server.config

open System.IO
open System.Xml

type DatabaseConfig =
    { Host: string
      Port: int
      Username: string
      Password: string
      DatabaseName: string }

type EmailConfig =
    { Host: string
      Port: int
      Domain: string
      SenderName: string }

module Config =
    let defaultConfig =
        @"
<config>
        <database>
                <host>127.0.0.1</host>
                <port>3306</port>
                <user>camblms</user>
                <password>V3l3tlen_J3lsz0</password>
                <name>camblogistics</name>
        </database>
        <email>
                <host>localhost</host>
                <port>25</port>
                <domain>localhost</domain>
                <sender>CambLMS</sender>
        </email>
</config>"

    let readDatabase () =
        let xmlDocument = new XmlDocument()

        try
            xmlDocument.Load("config.xml")
        with _ ->
            xmlDocument.LoadXml(defaultConfig)

        let databaseConfig = xmlDocument.SelectSingleNode("/config/database")

        { Host = databaseConfig.SelectSingleNode("host").InnerText
          Port = int <| databaseConfig.SelectSingleNode("port").InnerText
          Username = databaseConfig.SelectSingleNode("user").InnerText
          Password = databaseConfig.SelectSingleNode("password").InnerText
          DatabaseName = databaseConfig.SelectSingleNode("name").InnerText }

    let readEmail () =
        let xmlDocument = new XmlDocument()

        try
            xmlDocument.Load("config.xml")
        with _ ->
            xmlDocument.LoadXml(defaultConfig)

        let emailConfig = xmlDocument.SelectSingleNode("/config/email")

        { Host = emailConfig.SelectSingleNode("host").InnerText
          Port = int <| emailConfig.SelectSingleNode("port").InnerText
          Domain = emailConfig.SelectSingleNode("domain").InnerText
          SenderName = emailConfig.SelectSingleNode("sender").InnerText }

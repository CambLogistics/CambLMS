# CambLogistics Management System
This is the administration system for the virtual (in-game) logistics company CambLogistics
## Status
We are in alpha stage, bugs are present and some features are missing. See *TODO.md*
## Compilation
- Create a database in MySQL and run the contents of *basedb.sql* on it.

- Change the default connection string in *Database.fs* as needed.

- Run `dotnet publish --configuration Release` to build the project.
## Runtime setup
- Set the database connection and SMTP details in *config.xml*

- Run CambLMS using the binary or the .NET runtime on your system.

- Register an user and set its accepted column to 1(true) and its role ID to 14 with a database client.
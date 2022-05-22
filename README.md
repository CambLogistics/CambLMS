# CambLogistics Management System
This is the administration system for the virtual (in-game) logistics company CambLogistics
## Status
We are reaching alpha stage, bugs are present.
## Compiling and setup
The default database can be created by running *basedb.sql* on a MySQL (compatible) database server.

Register an user and set its accepted column to 1 with a database client. There will be an easier way to setup this, but not for now:)

Adjust the connection string in *Database.fs* before compiling! The runtime connection string can be set in *db.conf*

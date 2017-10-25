# RemoveSqlitePassword
Remove known password from System.Data.SQLite database file

# Notice
1. The project uses System.Data.SQLite dlls and embed these dlls as Embedded resource - that's why licensing is not explicitly stated.
2. the project uses Fody.Costura to embed these dlls into the main EXE file.

# Usage examples:
## Remove password from file:
rsp.exe "TargetDb.Sqlite" "passwordToRemove"

## Remove password from file in param, but use password in config file
rsp.exe "TargetDb.sqlite"

## Remove password from first SQLite3 file in same directory, and password from config file
rsp.exe

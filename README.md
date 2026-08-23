# Teron SQL Database Editor (TSDE)

A powerful database editing tool. Works with any SQL database.

## Features

- Connect to and browse multiple database connections (schemas, tables, columns) in a tree view
- Run custom SQL queries across multiple tabs, or auto-generate a query from a tree selection
- Export query results as CSV, SQL (`INSERT` statements), XML, or JSON
- Import/export raw `.sql` query files

## Requirements

- Windows
- [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) (or the SDK, for building from source)

## Building from source

```sh
dotnet build TeronSQLDatabaseEditor.slnx -c Release
```

## Data storage

Saved connections (`loginconfig.xml`) and crash logs (`error.log`) live under
`%LocalAppData%\TeronSQLDatabaseEditor\`. Saved passwords are encrypted at rest (Windows DPAPI,
current-user scope).

See [CHANGELOG.md](CHANGELOG.md) for version history.

## License

GPL-3.0 - see [LICENSE.txt](LICENSE.txt).

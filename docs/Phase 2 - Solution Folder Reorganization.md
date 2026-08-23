# Phase 2 - Solution Folder Reorganization

**Date:** 2026-08-23

## What was done

Per the user's request to bring the folder structure in line with how they organize their
other projects (`Views/`/`ConfigService.cs`-style separation used elsewhere in the portfolio),
the previously flat file layout was split:

- **`Forms/`** - all five WinForms (`AppForm`, `AppDelete`, `AppLogin`, `DBCEditor`, `Startup`),
  each with its `.cs`/`.Designer.cs`/`.resx` triplet. This is the WinForms analog of a WPF
  app's `Views/` folder.
- **`Services/`** - the former `Program.cs` was a single ~1,700-line file holding the entry
  point plus five unrelated classes. Split into one file per class:
  - `Globals.cs` - shared static state (`Logic.Globals`)
  - `HelperFunctions.cs` - UI/data helper methods
  - `DataRequest.cs` - the SQL schema/table/column/item fetch logic
  - `ExportQueryResults.cs` - CSV/SQL/XML/JSON export
  - `App.cs` - connection authentication/persistence
  - `AppInfo.cs` - new helper (see Phase 4) exposing `DisplayName`/`Version` from assembly
    metadata
  - `Program.cs` stays at the project root (conventional location for the entry point), now
    containing only the `Main` method plus the crash-logging/single-instance setup (Phase 6).
- No namespaces were changed (`Logic`, `AzureEditor`, `Database`, `Startup` all stay as they
  were) and no public API/identifier was renamed - this was a pure file-layout change to keep
  risk low, not a logic refactor.
- Updated `TeronSQLDatabaseEditor.csproj`'s explicit `<Compile>`/`<EmbeddedResource>` item
  lists to the new paths (`EnableDefaultCompileItems` is `false`, so nothing is auto-globbed;
  every moved file had to be re-listed).

## Not touched

- `Properties/` (assembly resources) and `Resources/` (`Definitions/`, `Icons/` content files)
  were already sensibly organized and left as-is.
- `redist/mariadb-connector-cpp-1.1.8-src/` - flagged in Phase 0 as unreferenced vendored code;
  left in place pending the user's decision.

## Verification

`dotnet build` succeeded immediately after the file moves and `.csproj` update, with the same
two pre-existing warnings as before the reorganization - confirming no reference was broken by
the move.

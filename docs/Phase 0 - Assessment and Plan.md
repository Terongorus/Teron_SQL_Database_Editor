# Phase 0 - Assessment and Plan

**Date:** 2026-08-23

## Starting state

The project's `.csproj` was already an SDK-style project targeting `net10.0-windows`
(`TeronSQLDatabaseEditor.csproj`), but the .NET Framework -> .NET 10 conversion had been left
half-finished:

- `packages.config` and a legacy `packages/` folder (old-style NuGet, `net481` targets) still
  present alongside the modern `PackageReference` items.
- `App.config` still carried .NET Framework-era `<supportedRuntime>` / `<bindingRedirect>`
  entries that do nothing under the modern .NET runtime.
- `Properties/AssemblyInfo.cs` existed but was **not included in any `<Compile>` item**
  (`EnableDefaultCompileItems` is `false`), so it was silently dead code.
- No `.sln`/`.slnx` mismatch - project still used the legacy `.sln` format.
- All WinForms source files sat flat in the repo root; `Program.cs` contained the entry point
  plus five unrelated classes (`Globals`, `HelperFunctions`, `DataRequest`,
  `ExportQueryResults`, `App`) in one ~1,700-line file.
- No git repository at all (confirmed - no `.git`, no GitHub remote under `Terongorus/*`).
- No LICENSE, README, CHANGELOG, `.gitignore`, or `.editorconfig`.
- No window title, no crash logging, no single-instance guard, no installer pipeline.
- Saved connections (`loginconfig.xml`) were read/written via a path relative to the working
  directory, not `%LocalAppData%`.
- `redist/mariadb-connector-cpp-1.1.8-src/` - a ~5.7 MB vendored C++ source tree, unreferenced
  by anything in the C# project (confirmed via grep - no `mariadb` usage anywhere in `.cs`
  files). Left untouched; flagged for the user to decide on rather than deleted unilaterally.

## Plan

1. **Phase 1** - finish the .NET 10 SDK modernization (remove the dangling Framework-era
   artifacts, restore proper SDK-generated assembly metadata, migrate `.sln` -> `.slnx`).
2. **Phase 2** - reorganize the flat file layout into `Forms/` and `Services/`, per the user's
   request to match how they organize other projects.
3. **Phase 3** - identity compliance (ARCHIVIST naming convention; license explicitly deferred
   per the user's instruction mid-session).
4. **Phase 4** - display/title-bar compliance (INSIGNIA).
5. **Phase 5** - process convention (CODEX: CHANGELOG, versioning, auto-incrementing build
   number).
6. **Phase 6** - runtime behavior (RESIDENCY: `%LocalAppData%` storage; SENTINEL: crash
   logging, single-instance guard, `.editorconfig`).
7. **Phase 7** - packaging (DISSEMINATE: publish profiles + Inno Setup wiring).
8. **Phase 8** - verification (TRIAL: build, publish, launch, crash-log, single-instance
   checks).
9. **Phase 9** - release (LAUNCH) - explicitly deferred; requires the user's separate
   go-ahead per standing convention, even inside an explicitly-invoked DOCTRINE run.

Each phase is logged in its own file in this folder as it's completed.

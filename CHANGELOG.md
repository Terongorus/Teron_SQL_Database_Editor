# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Versions follow major.minor.hotfix (e.g. 1.2.3).

## [2.0.0] - 2026-08-23

### Changed

- Migrated from .NET Framework 4.8.1 to .NET 10, finishing a partially-completed SDK-style
  conversion: removed the legacy `packages.config`/`packages/` folder and the .NET
  Framework-era `App.config` binding redirects, and replaced the unused `AssemblyInfo.cs`
  with SDK-style assembly metadata in the `.csproj`.
- Reorganized the project layout: WinForms moved into `Forms/`, business/data logic split out
  of the former monolithic `Program.cs` into individual files under `Services/`.
- Migrated the Visual Studio solution file from `.sln` to the newer `.slnx` format.
- Saved connections (`loginconfig.xml`) now live under `%LocalAppData%\TeronSQLDatabaseEditor\`
  instead of a path relative to the working directory. **If you have an existing
  `loginconfig.xml` next to the installed `.exe`, copy it to
  `%LocalAppData%\TeronSQLDatabaseEditor\loginconfig.xml`** to keep your saved connections.

### Added

- Main window title now reads "Teron SQL Database Editor v\<version\>", read live from
  assembly metadata.
- Crash logging: unhandled exceptions are now appended to
  `%LocalAppData%\TeronSQLDatabaseEditor\error.log`.
- Single-instance guard: launching a second copy now shows a notice and exits instead of
  running two instances side by side.
- Auto-incrementing 4th build-number version component, tracked via `BuildNumber.txt`.
- Licensed under GPL-3.0 (see `LICENSE.txt`).

### Security

- Saved connection passwords are now encrypted at rest (Windows DPAPI, current-user scope)
  before being written to `loginconfig.xml`, instead of stored as plain text. An existing
  plaintext `loginconfig.xml` carried over from an older version still loads correctly.

### Fixed

- Adding the very first saved connection no longer crashes with a missing-file error (the old
  code always tried to load `loginconfig.xml` before checking whether it existed yet).
- Removed two unused fields (`Globals.app_login`, `Globals.app_delete`) that were declared but
  never assigned or read anywhere, eliminating the corresponding compiler warnings.

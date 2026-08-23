# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Versions follow major.minor.hotfix (e.g. 1.2.3).

## [3.0.1] - 2026-08-24

### Added

- Automatic build-number tracking (4th version component) and a consolidated `Build\` output
  directory for all build artifacts (regular builds, intermediates, and publish output), matching
  the rest of this developer's app portfolio.

## [3.0.0] - 2026-08-23

UI rewrite from Windows Forms to WPF, following this developer's portfolio-wide move away from
WinForms. Business logic (SQL querying, DBC file parsing, connection storage) is unchanged -
only the UI layer and its control APIs were translated.

### Changed

- All five windows (Startup menu, Azure SQL Editor, Warcraft DBC Editor, add/delete-connection
  dialogs) rewritten in WPF/XAML.
- The custom owner-drawn closeable query tabs (hand-drawn "x" button with manual hit-testing)
  are now real WPF buttons built into each tab's header - same close behavior, no pixel-math.
- DBC file filtering now applies directly to the underlying data view instead of swapping data
  sources, with no behavior change to the filter expressions themselves.
- Query editor tabs and log panes are plain multi-line text boxes rather than WPF's
  FlowDocument-based `RichTextBox` - this app never used any rich-text formatting, so the
  simpler control is a faithful translation, not a feature reduction.

### Fixed

- Installer packaging (`dotnet publish`) now finds Inno Setup when it's installed under
  `%LocalAppData%\Programs\Inno Setup 6\` (previously it only checked `Program Files`), and the
  path handed to the Inno Setup compiler is no longer silently wrong, which had made every
  publish since the app moved into this repo's own installer folder skip building an installer
  entirely.
- Query/results tabs are now correctly announced with their real name ("Query 1", "Query 2",
  etc.) to screen readers and other assistive/automation tools, instead of an internal object
  dump.

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

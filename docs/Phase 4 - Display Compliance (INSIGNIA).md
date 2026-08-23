# Phase 4 - Display Compliance (INSIGNIA)

**Date:** 2026-08-23

## Findings

No window in the app set a real title before this phase. The actual main window (the one
passed to `Application.Run` - `Startup`, not `AppForm`) had no `Text` assignment anywhere and
was showing the WinForms default (`Form1`), confirmed by an actual launch-and-inspect test
before this fix (see Phase 8). `AppDelete`'s dialog title was a leftover dev placeholder
(`"AppDelete"`), and `DBCEditor` had no title either.

## What was done

- Added `Services/AppInfo.cs` - a small static helper (matching the WPF-app convention, adapted
  for WinForms) exposing:
  - `DisplayName` - reads `AssemblyProductAttribute`, strips everything from `" ("` onward to
    drop the `(TSDE)` abbreviation suffix for on-screen use.
  - `Version` - reads `AssemblyInformationalVersionAttribute` (not `AssemblyVersion`, which the
    CLR always pads to 4 parts regardless of what's configured).
  - `DisplayNameWithVersion` - `"<DisplayName> v<Version>"`.
- `<IncludeSourceRevisionInInformationalVersion>false</IncludeSourceRevisionInInformationalVersion>`
  added to the `.csproj` so the informational version never gets a `+<git-sha>` suffix appended.
- **`Startup`** (the real main/first window - a launcher menu that opens either `AppForm` or
  `DBCEditor`): title set to `AppInfo.DisplayNameWithVersion`, e.g.
  `"Teron SQL Database Editor v2.0.0.7"`.
- **`AppForm`** (SQL editing mode): title set to `"SQL Editor - {AppInfo.DisplayNameWithVersion}"`,
  using the context-prefix pattern from the established convention.
- **`DBCEditor`** (DBC editing mode): title set to `"DBC Editor - {AppInfo.DisplayNameWithVersion}"`.
- **`AppDelete`**: placeholder title `"AppDelete"` replaced with `"Delete Connection"`.
- **`AppLogin`**: already had a real title (`"New Connection Creation"`) - left unchanged.

## Verification

Published a self-contained build, launched it, and read the real `MainWindowTitle` via
PowerShell rather than trusting the source alone - confirmed
`"Teron SQL Database Editor v2.0.0.7"` on the actual running window. See Phase 8 for the full
launch-test log.

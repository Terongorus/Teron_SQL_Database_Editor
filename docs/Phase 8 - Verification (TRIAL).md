# Phase 8 - Verification (TRIAL)

**Date:** 2026-08-23

Checklist-style summary of what was actually verified, not just "compiles."

## Build

- `dotnet build TeronSQLDatabaseEditor.csproj -c Debug` - succeeded after every phase's
  changes, re-run repeatedly through the session. Two pre-existing warnings throughout
  (`CS0649` on `Globals.app_login`/`Globals.app_delete`, never assigned - a pre-existing issue,
  unrelated to this work, left as-is).
- `dotnet build TeronSQLDatabaseEditor.slnx` - confirmed equivalent to building the `.csproj`
  directly after the `.sln` -> `.slnx` migration.

## Publish / installer pipeline

- `dotnet publish -p:PublishProfile=win-x64` succeeded; self-contained single-file exe + copied
  `Resources/` confirmed on disk. Installer step gracefully skipped with a warning (Inno Setup
  not installed on this machine) rather than failing the build - see Phase 7.

## Launch-time behavior (real launches, not just source review)

- **Title bar (INSIGNIA):** first attempt showed `"Form1"` - caught because the title was set
  on `AppForm` but the real main window (passed to `Application.Run`) is `Startup`. Fixed, then
  re-verified: launched `bin\Debug\net10.0-windows\TeronSQLDatabaseEditor.exe`, read the live
  `Process.MainWindowTitle` via PowerShell, confirmed
  `"Teron SQL Database Editor v2.0.0.7"`.
- **Single-instance guard (SENTINEL):** launched the exe twice back-to-back; confirmed via
  `MainWindowTitle` that the second process's visible window was the "already running" message
  box (titled `"Teron SQL Database Editor"`), not a second copy of the main app window.
- **Crash logging (SENTINEL):** temporarily added a real `throw new
  InvalidOperationException(...)` to `Startup_Load`, rebuilt, launched, and confirmed
  `%LocalAppData%\TeronSQLDatabaseEditor\error.log` was actually written with a full stack
  trace - not just that the exception hooks compiled. Reverted the test throw immediately after
  and rebuilt clean. This closes a gap explicitly flagged as open in the robustness convention
  ("crash logging has only ever been verified as 'the hooks are wired up'").

## Not verified this session

- **RESIDENCY data-folder behavior on a truly fresh machine** (no prior `%LocalAppData%`
  folder) - only exercised indirectly via the crash-log test above, which does create the
  folder. Adding an actual saved connection through the UI (exercising
  `AuthenticateUser`/`LoadConnectionDetails`) was not interactively tested.
- **Installer silent install/uninstall** - blocked on Inno Setup not being installed on this
  machine (see Phase 7).
- **LAUNCH** (release cut, tag, GitHub release) - not started; requires the user's explicit
  separate go-ahead per standing convention.

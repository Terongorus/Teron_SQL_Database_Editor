# Phase 7 - Packaging (DISSEMINATE)

**Date:** 2026-08-23

## What was done

Wired installer creation into `dotnet publish` itself, so publishing is the only manual step
needed to produce a distributable installer:

- `Properties/PublishProfiles/win-x64.pubxml` and `win-x86.pubxml` - self-contained,
  single-file, `PublishReadyToRun=false`.
- `Installer/Setup.iss` - an Inno Setup script. Install directory is
  `{autopf}\TeronSQLDatabaseEditor` (no spaces/apostrophes, per convention). Output filename
  pattern: `TeronSQLDatabaseEditorSetup-<arch>.exe`, landing in `bin\InstallerPackage\`
  (sibling of `Debug`/`Release`, not nested under `bin\Publish\`).
- `TeronSQLDatabaseEditor.csproj`: a `BuildInstaller` target, `AfterTargets="Publish"`. Computes
  the RID -> Inno arch mapping and searches common Inno Setup install locations **inside the
  target body** (not a top-level `PropertyGroup` - `$(RuntimeIdentifier)` isn't populated until
  the publish pipeline is actually running). If Inno Setup isn't found, emits an MSBuild
  `<Warning>` and skips packaging rather than failing the build - a plain publish still succeeds
  on a machine without the tool installed.
- App version is passed into the installer as a preprocessor define
  (`/DMyAppVersion=$(FileVersion)`) so it can't drift from the app's own version.
- `.gitignore` explicitly un-ignores `Properties/PublishProfiles/*.pubxml` (the default VS
  `.gitignore` pattern blanket-excludes `*.pubxml`, which would otherwise silently break "a
  fresh clone can publish" - a documented incident on other repos in this portfolio).

## Verification

Ran `dotnet publish -p:PublishProfile=win-x64` (authorized inside this explicitly-invoked
DOCTRINE run per the standing "ask before publish" convention's own carve-out for TRIAL
checkpoints). Result:

- Publish succeeded: self-contained single-file `TeronSQLDatabaseEditor.exe` (~126 MB, expected
  for a self-contained single-file WinForms app) plus `Resources/` copied correctly.
- The `BuildInstaller` target ran and correctly emitted:
  `warning : Skipping installer packaging: Inno Setup (ISCC.exe) not found. Install it from
  https://jrsoftware.org/isinfo.php to enable automatic installer creation.`
  (Inno Setup is not installed on this machine - confirmed by checking both standard install
  paths before writing the target.) This is the designed graceful-skip behavior, not a failure.
- **Not yet verified end-to-end:** the actual `ISCC.exe` invocation, and a real silent
  install/uninstall test, since Inno Setup isn't installed here. If/when Inno Setup is
  installed, re-run `dotnet publish -p:PublishProfile=win-x64` and confirm
  `bin\InstallerPackage\TeronSQLDatabaseEditorSetup-x64.exe` is produced and installs cleanly.

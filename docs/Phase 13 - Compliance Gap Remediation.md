# Phase 13 - Compliance Gap Remediation

**Date:** 2026-08-24

A follow-up compliance re-check of the WPF port (Phase 12) found the consolidated build-output
convention (`Directory.Build.props`) had never actually been applied to this app - a pre-existing
gap, not something the port itself regressed - plus two missing csproj properties.

## Findings and fixes

- **`Directory.Build.props` was missing entirely.** Added the standard block (consolidated
  `Build\<Configuration>\TeronSQLDatabaseEditor\` output, `Build\obj\...` intermediates,
  `Build\Publish\...` for plain `dotnet build`). Added `/Build/` to `.gitignore`. Updated both
  `Properties/PublishProfiles/win-x64.pubxml` and `win-x86.pubxml`'s `PublishDir` from
  `bin\Publish\win-x64\`/`win-x86\` to explicit `Build\Publish\TeronSQLDatabaseEditor\win-x64\`/
  `win-x86\` paths, so the profile's own explicit value wins over `Directory.Build.props`'s
  generic (RID-collapsing) formula for real publishes. `Installer/Setup.iss` reads `PublishDir`
  via the csproj's own `/DPublishDir=...` compiler define rather than a separate hardcoded path,
  so no change was needed there - it picks up the new location automatically.
- **`ImplicitUsings`/`LangVersion` were missing** from the csproj (only `Nullable` was set) -
  added both, matching the rest of the portfolio's convention.

## Not changed, and why

The app's entry point (`Program.cs`, in the `Logic` namespace) constructs a WPF `Application`
manually rather than using an `App.xaml`/`App.xaml.cs` pair. All three required crash hooks
(`DispatcherUnhandledException`, `AppDomain.CurrentDomain.UnhandledException`,
`TaskScheduler.UnobservedTaskException`) and the single-instance mutex guard are already present,
correctly named, and correctly placed before any window is created - functionally this is fully
compliant with the runtime-robustness convention, it just doesn't follow the *typical* WPF shape
(`App.OnStartup`/`OnExit`) the convention describes as an example. Restructuring a working
production app's entry point purely for architectural conformity, with no functional gap to fix,
was judged not worth the risk - left as-is.

## Version

Bumped `3.0.0` -> `3.0.1` (hotfix - build-tooling only, no new user-facing capability), synced
across `TeronSQLDatabaseEditor.csproj`'s `MajorMinorPatchVersion` and `Installer/Setup.iss`'s
`MyAppVersion` fallback.

## Verification

`dotnet build TeronSQLDatabaseEditor.csproj -c Debug` and a clean `-c Release` rebuild (deleting
`Build\` first, per the known WPF config-switch gotcha) - both 0 warnings, 0 errors, output
correctly landing under `Build\Debug\TeronSQLDatabaseEditor\` / `Build\Release\...`.

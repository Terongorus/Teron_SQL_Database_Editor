# Phase 5 - Process Convention (CODEX)

**Date:** 2026-08-23

## What was done

- Created `CHANGELOG.md` (none existed before) in Keep a Changelog format, `major.minor.hotfix`
  versioning.
- First entry: **`[2.0.0] - 2026-08-23`**. A .NET Framework -> modern .NET migration is always a
  MAJOR bump per established cross-repo precedent (`Teron_Email_Client` used `2.0.0` for the
  same kind of change), even though most of the underlying SDK conversion had technically
  already happened before this session - the CHANGELOG entry covers finishing that migration
  plus the folder reorg, RESIDENCY path change, and the new INSIGNIA/SENTINEL behavior, all
  landing together as one release-worthy jump.
- Added the standing auto-incrementing 4th build-number version component to the `.csproj`:
  `<MajorMinorPatchVersion>2.0.0</MajorMinorPatchVersion>` + `BuildNumber.txt` +
  `IncrementBuildNumber` target (`BeforeTargets="BeforeBuild"`), copied verbatim from the
  established convention. `BuildNumber.txt` is tracked in git (not ignored), so the counter
  persists across clones instead of resetting.
- Created `README.md` (none existed before) - feature summary, requirements, build
  instructions, and a note on where app data lives.

## Not done

- No GitHub remote exists yet, so there's nothing to compare a version-in-README link against.
- License section deliberately omitted from README - see Phase 3.

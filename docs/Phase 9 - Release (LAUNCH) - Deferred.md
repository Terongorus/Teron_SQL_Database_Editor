# Phase 9 - Release (LAUNCH) - Deferred

**Date:** 2026-08-23

Not started. Per standing convention, LAUNCH (merge `dev` -> `release`, tag, rebuild
installers from the tagged commit, `gh release create`) always requires its own explicit
go-ahead, even when it's the final phase of an explicitly-invoked DOCTRINE run - "bring this
into compliance" is not itself authorization to cut a release.

Additional blockers specific to this project, beyond the general LAUNCH gate:

- ~~No GitHub remote exists yet~~ - **resolved in Phase 11**: `Terongorus/Teron_SQL_Database_Editor`
  now exists (public), `release`+`dev` both pushed, `release` set as the default branch.
- ~~No license file~~ - **resolved in Phase 10**: GPL-3.0 `LICENSE.txt` added.
- **Installer not end-to-end verified** - Inno Setup isn't installed on this machine (Phase 7);
  the packaging step has only been verified up to its graceful-skip warning path. Still open.

With the GitHub-remote and license blockers cleared, the only things standing between this
project and an actual LAUNCH are: the user's own explicit go-ahead (never implied by DOCTRINE
alone) and end-to-end installer verification once Inno Setup is available.

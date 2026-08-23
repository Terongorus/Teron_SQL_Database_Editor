# Phase 9 - Release (LAUNCH) - Deferred

**Date:** 2026-08-23

Not started. Per standing convention, LAUNCH (merge `dev` -> `release`, tag, rebuild
installers from the tagged commit, `gh release create`) always requires its own explicit
go-ahead, even when it's the final phase of an explicitly-invoked DOCTRINE run - "bring this
into compliance" is not itself authorization to cut a release.

Additional blockers specific to this project, beyond the general LAUNCH gate:

- **No GitHub remote exists yet.** LAUNCH assumes a repo already published to
  `github.com/Terongorus/...`; this project has never been pushed anywhere. Creating that
  remote and pushing are their own separate, visible-to-others actions.
- **No license file** - deferred per explicit instruction (Phase 3). Worth resolving before any
  public release.
- **Installer not end-to-end verified** - Inno Setup isn't installed on this machine (Phase 7);
  the packaging step has only been verified up to its graceful-skip warning path.

See the session's closing summary for the full list of open items and questions for the user.

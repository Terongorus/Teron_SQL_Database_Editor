# Phase 6 - Runtime Behavior (RESIDENCY + SENTINEL)

**Date:** 2026-08-23

## RESIDENCY - app data storage location

**Before:** `Globals.login_config = @"loginconfig.xml";` - a path relative to the current
working directory, not `%LocalAppData%`. This is exactly the pattern flagged as risky in the
storage-location convention (assumes the working directory is writable, not per-user-safe).

**After:**
```csharp
public static readonly string AppDataRoot = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "TeronSQLDatabaseEditor");
public static string login_config = Path.Combine(AppDataRoot, "loginconfig.xml");
```
`Directory.CreateDirectory(Globals.AppDataRoot)` added before the one write site
(`Services/App.cs`, `AuthenticateUser`).

**Bug fixed incidentally:** `AuthenticateUser` unconditionally called
`XDocument.Load(Globals.login_config)`, which throws if the file doesn't exist yet - meaning
adding the very first saved connection on a fresh install would have crashed. Fixed by checking
`File.Exists` first and starting from an empty `<Connections/>` document when it doesn't. This
was directly exposed by moving to a real `%LocalAppData%` folder (no stray `loginconfig.xml`
would ever pre-exist there the way one might have in a working directory from manual testing),
so it's treated as part of this same fix rather than a separate change.

**Migration note (in CHANGELOG.md):** existing users with a `loginconfig.xml` next to the old
installed `.exe` need to copy it to `%LocalAppData%\TeronSQLDatabaseEditor\loginconfig.xml`
manually - no automatic migration code was written, per the standing "flag it in the changelog,
don't build migration logic preemptively" convention.

## SENTINEL - crash logging, single-instance guard, `.editorconfig`

- **Crash logging:** `Program.cs` now wires `Application.SetUnhandledExceptionMode`,
  `Application.ThreadException`, `AppDomain.CurrentDomain.UnhandledException`, and
  `TaskScheduler.UnobservedTaskException` (calling `SetObserved()`) to a `LogException` helper
  that appends to `%LocalAppData%\TeronSQLDatabaseEditor\error.log`, wrapped in its own
  try/catch so logging itself can never crash the app.
- **Single-instance guard:** a named `Mutex(true, "TeronSQLDatabaseEditor.SingleInstance", out
  createdNew)` at the very top of `Main()`, before any window is created. A second launch shows
  a `MessageBox` ("Teron SQL Database Editor is already running.") and exits without creating a
  window.
- **`.editorconfig`:** added at the repo root - 4-space indent (2 for
  json/xml/xaml/csproj/slnx/config/resx), UTF-8, CRLF, Allman braces, trim trailing whitespace,
  final newline. Formatting-only, no naming-rule enforcement (existing snake_case identifiers
  like `login_list`/`connection_string` are left alone).

## Verification

Both pieces were verified against **real behavior**, not just "the hooks compile" (closing a
gap explicitly flagged as open in the robustness convention):

- Temporarily added a real `throw` in `Startup_Load`, rebuilt, launched the exe, and confirmed
  `error.log` was actually written with a full stack trace - then reverted the test throw.
- Launched the exe twice in a row and confirmed the second process's window was the
  "already running" `MessageBox`, not a second copy of the app.

Full logs are in Phase 8.

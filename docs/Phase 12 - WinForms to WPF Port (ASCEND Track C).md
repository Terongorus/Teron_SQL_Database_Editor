# Phase 12 - WinForms to WPF Port (ASCEND Track C)

**Date:** 2026-08-23

## Starting state

This app had already completed a full non-UI DOCTRINE pass earlier the same day as WinForms
(Phases 0-11: .NET 10 SDK modernization, solution reorg, ARCHIVIST/INSIGNIA/CODEX/RESIDENCY/
SENTINEL, DISSEMINATE, TRIAL, GitHub repo creation - LAUNCH deferred). This pass ports the UI
layer only, per the user's portfolio-wide retroactive move off WinForms (see
[[dotnet10-modernization-procedure]] (ASCEND) Track C) - second in the confirmed worst-to-least-
by-size queue (`Teron_Raid_Manager` first, this app second, `Teron_Doom_Launcher` next).

Five forms ported: `Startup` (menu), `AppLogin` (add-connection dialog), `AppDelete`
(remove-connections dialog), `AppForm` (the Azure SQL editor - by far the largest: toolbar,
tree view, dynamic query tabs, results grid, log tabs, three context menus), `DBCEditor` (the
Warcraft DBC editor - a from-scratch binary-format parser/editor with dynamic per-file tabs,
each hosting a filterable data grid).

## Control/API translation

Same core mechanical translations established on `Teron_Raid_Manager` (`Form`→`Window`,
`MessageBoxButtons`/`Icon`→`MessageBoxButton`/`Image`, WinForms `Timer`→`DispatcherTimer`,
`TabPage`→`TabItem`, etc.), plus several new ones specific to this app:

- **`RichTextBox`→plain `TextBox`, not WPF `RichTextBox`.** Swept the whole codebase first for
  any actual rich-text usage (`SelectionColor`/`SelectionFont`/`.Rtf`/etc.) - found none. Every
  `RichTextBox` in this app (the query editor tabs, `messages_log_textbox`, DBCEditor's
  errors/warnings/messages boxes) was really just a plain multi-line text control. WPF's
  `RichTextBox` is FlowDocument-based with no `.Text` property at all, so forcing that
  translation would have meant rewriting every read/write site through `TextRange` for no
  behavioral gain - `TextBox` (`AcceptsReturn`, `TextWrapping`, scrollbars) is the faithful,
  much simpler translation here.
- **`DataGridView`→WPF `DataGrid`**, bound to a `DataTable`'s `.DefaultView` (a `DataView`)
  instead of the `DataTable` directly, so **filtering uses `DataView.RowFilter` in place of a
  WinForms `BindingSource.Filter` wrap/restore dance** - simpler, since the `DataGrid` re-renders
  live off the same `DataView` with no data-source swap needed. `DataGridViewTextBoxColumn` with
  `DataPropertyName` became `DataGridTextColumn` with a `Binding` on `[FieldN]` (WPF's supported
  indexer-binding syntax against a `DataRowView`).
- **Owner-drawn closeable tabs (`query_tab_control`'s custom "x" hit-testing via `GetTabRect` +
  `MouseMove`/`MouseDown`/`DrawItem`) replaced with a real WPF close button per tab**, built in
  `HelperFunctions.AddNewTab` (a `StackPanel` with a `TextBlock` + small `Button` in the
  `TabItem.Header`). WPF has no owner-draw equivalent for `TabControl`; a real button with its
  own hover state is the idiomatic replacement and removes an entire class of pixel-math bugs.
  `AutomationProperties.Name` is set explicitly on each tab (see Bug found, below) since the
  `Header` is no longer a plain string.
- **WinForms `TreeNode.Level`/`.Text`/`.Parent` chain has no WPF `TreeViewItem` equivalent** -
  replaced with a small `SqlTreeNodeTag` class (`Text`/`Level`/`Parent`) attached via
  `TreeViewItem.Tag`, populated when `PopulateTreeViewFromDatabase` builds the tree. Every
  `node.Level`/`.Parent?.Text` call site translates directly onto this tag.
- **Native file dialogs**: `System.Windows.Forms.OpenFileDialog`/`SaveFileDialog` →
  `Microsoft.Win32.OpenFileDialog`/`SaveFileDialog` (the WPF-native equivalents, no WinForms
  reference needed at all in the final app). `ShowDialog()` returns `bool?` instead of
  `DialogResult`; the `FileOk` event exists on both with the same signature, so `DBCLogic
  .SelectFile()`'s pre-read-validation pattern (prompt for client version inside `FileOk`, cancel
  the dialog if the user backs out) carried over unchanged.
- **Two small code-only dialogs** (`PromptForClientVersion`'s inline version-picker,
  `FilterForm`'s column/operator/value filter builder) were originally built as plain WinForms
  `Form`s constructed entirely in C# (no designer). Both are now plain WPF `Window`s built the
  same way (a `Grid` with manually-positioned children) - same "no XAML file" approach the
  original used, just the WPF control types.
- **Manual `ContextMenuStrip.Show(control, location)` calls** (`HelperFunctions
  .MouseButtonDetect`, dispatched from a shared `MouseClick` handler wired to three different
  controls) replaced with WPF's native `ContextMenu` property, attached directly to `data_viewer`
  and `connections_tree_view` in `AppForm`'s constructor. `DBCEditor`'s per-tab right-click menu
  (close current/close all) is instead built per-tab in `CreateNewDataGrid`, wired directly to
  that `TabItem` - simpler than hit-testing which tab was clicked.

## Disclosed simplifications

- Dropped `DataGridView`'s `MouseWheel` handler that mapped Shift+scroll to horizontal scrolling
  and Ctrl+scroll to jumping 3 rows vertically (`AppForm`'s data grid didn't have this; only
  DBCEditor's dynamically-created grids did). WPF `DataGrid`'s built-in horizontal scrollbar
  already covers the Shift+scroll use case in practice, and the exact `FirstDisplayedScrollingRowIndex`-jump behavior has no direct WPF equivalent worth the added complexity.
- The original's third context menu (`query_tab_control_context_menu`, a single dead "Rename"
  item) was never actually reachable in the WinForms version either - its
  `MouseButtonDetect` branch compared the click event's `sender` to `query_tab_control.TabPages`,
  a collection that is never the actual sender of a `MouseClick` event. Not carried over.

## Bug found and fixed (this port, not pre-existing)

`HelperFunctions.AddNewTab`/`NameQueryTabs` originally set the `TabItem.Header` directly to a
`StackPanel` (title `TextBlock` + close `Button`). WPF's default `TabItem` automation peer falls
back to `Header.ToString()` for its accessible Name when `Header` isn't a plain string - so every
query tab's name was reported to UI Automation (and any screen reader) as a useless object dump
instead of "Query 1"/"Query 2". Found live during TRIAL (`AutomationElement` tab enumeration
returned `"System.Windows.Controls.TabItem Header: Content:"`). Fixed by setting
`AutomationProperties.SetName(tab, title)` explicitly in both `SetTabTitle` and `AddNewTab`,
kept in sync with the visible `TextBlock`.

## Bug found and fixed (pre-existing, unrelated to WPF, caught during TRIAL)

`TeronSQLDatabaseEditor.csproj`'s `BuildInstaller` target only checked `Program Files`/
`Program Files (x86)` for `ISCC.exe`, missing the `%LocalAppData%\Programs\Inno Setup 6\`
location this Inno Setup install actually lives at on this machine (the location the other
portfolio apps' equivalent targets already check first). Publishing silently skipped installer
creation with only a build warning. Fixed by adding the `$(LocalAppData)` check, matching
`Teron_Raid_Manager`/`Teron_Game_Launcher`'s pattern.

A second, related bug surfaced once the installer step started actually running: the `/D
PublishDir=$(PublishDir)` argument passed to `ISCC.exe` was relative to the *project* directory,
but Inno Setup resolves `[Files]` source paths relative to the `.iss` script's own directory
(`Installer\`), one level down - so the installer looked for the published exe at
`Installer\bin\Publish\win-x64\...` and failed with "Source file ... does not exist." Fixed by
passing an absolute path (`$(MSBuildProjectDirectory)\$(PublishDir)`), quoted correctly to avoid
the classic trailing-backslash-escapes-the-closing-quote gotcha (an even number of backslashes -
two - must precede the closing quote for the argument to parse as one literal trailing
backslash).

## Verification (TRIAL)

- Clean rebuild, both Debug and Release, 0 warnings / 0 errors, after a full `bin`/`obj`/`Build`
  delete.
- Both `win-x64` and `win-x86` publish profiles succeed; both installers now actually build
  (`TeronSQLDatabaseEditorSetup-x64.exe`/`-x86.exe`) after the two installer-pipeline fixes above.
- Checked for a pre-existing real install/RESIDENCY data before installing (per the updated
  [[testing-verification-procedure]] (TRIAL) step, added this session after a related incident on
  `Teron_Raid_Manager`) - both were empty/absent, safe to proceed.
- Silent install to `Program Files`, launched the installed copy directly (confirmed by PID that
  it started, stayed running, and showed the correct title), then silent uninstall - confirmed
  the install directory was fully removed. RESIDENCY test data folder cleaned up afterward.
- Live UI Automation pass (PID/AutomationId-scoped only, no synthetic input) across all three
  main windows:
  - `Startup`: both buttons render and correctly navigate to `AppForm`/`DBCEditor`.
  - `DBCEditor`: toolbar buttons render with correct tooltips; `Open File` triggers a real,
    functional `Microsoft.Win32.OpenFileDialog` (confirmed via raw `EnumWindows` - the dialog is a
    modal owned window that doesn't enumerate under `AutomationElement.RootElement.FindAll
    (TreeScope.Children, ...)`, which cost some time to track down but isn't an app bug); closing
    it cleanly leaves the app running and responsive.
  - `AppForm`: tree view/tab control/data grid/toolbar all render; `messages_log` tab shows the
    real `LoginMessage` text once selected (WPF `TabControl` only realizes the selected tab's
    content, as expected); `New Query` creates a second tab, and its close button removes it
    cleanly (the fix above confirmed both functionally and via the corrected automation name);
    `Add Connection` (a nested `MenuItem` under the `Connections` dropdown, which itself needed
    `ExpandCollapsePattern.Expand()` before its children became queryable) opens `AppLogin` with
    all controls present including the `PasswordBox`; `Cancel` closes it without disturbing the
    parent window.
- Not tested: an actual DB connection or DBC file load/save round-trip (no real SQL Server or
  hand-built binary-exact `.dbc` fixture available in this environment) - the parsing/DB logic
  itself is unchanged from the WinForms version either way, so this is a UI-layer port risk, not
  a data-correctness one.

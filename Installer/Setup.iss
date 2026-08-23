#ifndef MyAppVersion
  #define MyAppVersion "3.0.1.0"
#endif
#ifndef MyAppArch
  #define MyAppArch "x64"
#endif
#ifndef PublishDir
  #define PublishDir "..\bin\Publish\win-x64\"
#endif

#define MyAppName "Teron SQL Database Editor"
#define MyAppPublisher "Terongorus"
#define MyAppExeName "TeronSQLDatabaseEditor.exe"

[Setup]
AppId={{7C1B6C0E-6C6B-4B6C-9C7A-3B7F0F6E1A21}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\TeronSQLDatabaseEditor
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=..\bin\InstallerPackage
OutputBaseFilename=TeronSQLDatabaseEditorSetup-{#MyAppArch}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
#if MyAppArch == "x64"
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
#endif

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop icon"; GroupDescription: "Additional icons:"; Flags: unchecked

[Files]
Source: "{#PublishDir}{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#PublishDir}Resources\*"; DestDir: "{app}\Resources"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent

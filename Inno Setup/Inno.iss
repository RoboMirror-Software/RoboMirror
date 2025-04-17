#define MyAppName "RoboMirror"
#define MyAppVersion "1.2"
#define MyAppExeName "RoboMirror.exe"

[Setup]
OutputDir=..\Release
ArchitecturesInstallIn64BitMode=x64
AppId={{9B62D1DC-96D2-4F93-BC20-4EA9ADB0C230}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher=Martin Kinkelin
VersionInfoVersion={#MyAppVersion}
UninstallDisplayIcon={app}\{#MyAppExeName}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; Flags: unchecked

[Files]
Source: "..\Release\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Release\{#MyAppExeName}.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Release\Microsoft.Win32.TaskScheduler.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Release\{#MyAppName} web site.url"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Release\Tools\Robocopy.exe"; DestDir: "{app}\Tools"; Flags: ignoreversion
Source: "..\Release\Tools\vshadow32.exe"; DestDir: "{app}\Tools"; Flags: ignoreversion; Check: not Is64BitInstallMode
Source: "..\Release\Tools\vshadow64.exe"; DestDir: "{app}\Tools"; Flags: ignoreversion; Check: Is64BitInstallMode

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{#MyAppName} (Admin)"; Filename: "{app}\{#MyAppExeName}"; Check: IsWindowsVistaOrLater
Name: "{group}\{#MyAppName} web site"; Filename: "http://robomirror.sourceforge.net/"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
const
  CLSID_ShellLink = '{00021401-0000-0000-C000-000000000046}';
  STGM_READWRITE  = $2;
  SLDF_RUNAS_USER = $2000;

type
  IPersist = interface(IUnknown)
    '{0000010C-0000-0000-C000-000000000046}'
    function GetClassID(var classID: TGUID): HResult;
  end;

  IPersistFile = interface(IPersist)
    '{0000010B-0000-0000-C000-000000000046}'
    function IsDirty: HResult;
    function Load(pszFileName: String; dwMode: Longint): HResult;
    function Save(pszFileName: String; fRemember: BOOL): HResult;
    function SaveCompleted(pszFileName: String): HResult;
    function GetCurFile(out pszFileName: String): HResult;
    function GetFlags(out Flags: DWORD): HResult;
    function SetFlags(Flags: DWORD): HResult;
  end;

  IShellLinkDataList = interface(IUnknown)
    '{45E2B4AE-B1C3-11D0-B92F-00A0C90312E1}'
    function AddDataBlock(pDataBlock : DWORD) : HResult;
    function CopyDataBlock(dwSig : DWORD; var ppDataBlock : DWORD) : HResult;
    function RemoveDataBlock(dwSig : DWORD) : HResult;
    function GetFlags(var pdwFlags : DWORD) : HResult;
    function SetFlags(dwFlags : DWORD) : HResult;
  end;

// Tries to mark the specified shortcut as 'run-as admin' and returns true if successful.
function SetShortcutRunAsAdmin(const shortcutPath: String) : Boolean;
var
  obj: IUnknown;
  pf: IPersistFile;
  sldl: IShellLinkDataList;
  flags: DWORD;
begin
  Result := False;

  try
    obj := CreateComObject(StringToGuid(CLSID_ShellLink))
    
    // load the shortcut
    pf := IPersistFile(obj);
    OleCheck(pf.Load(shortcutPath, STGM_READWRITE));

    // set the SLDF_RUNAS_USER flag
    sldl := IShellLinkDataList(obj);
    OleCheck(sldl.GetFlags(flags));
    flags := flags or SLDF_RUNAS_USER;
    OleCheck(sldl.SetFlags(flags));

    // save the shortcut
    OleCheck(pf.Save(shortcutPath, False));

    Result := True
  finally
  end;
end;


// Indicates whether the specified version of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0/4.5 Client Profile
//    'v4\Full'       .NET Framework 4.0/4.5 Full Installation
function IsDotNetDetected(const version: String): Boolean;
begin
  Result := RegKeyExists(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version);
end;

function IsWindowsVistaOrLater: Boolean;
begin
  Result := GetWindowsVersion >= $06000000
end;


// INNO EVENTS

function InitializeSetup(): Boolean;
begin
  if not IsDotNetDetected('v2.0.50727') then begin
    MsgBox('{#MyAppName} requires Microsoft .NET Framework 2.0.'#13
           'Please install it first and then start this installer again.',
           mbInformation, MB_OK);
    Result := False;
  end else
    Result := True;
end;

procedure CurStepChanged(const curStep: TSetupStep);
var
  shortcutPath: String;
begin
  shortcutPath := ExpandConstant('{group}\{#MyAppName} (Admin).lnk');
  if (curStep = ssPostInstall) and FileExists(shortcutPath) then
    SetShortcutRunAsAdmin(shortcutPath);
end;


procedure CurUninstallStepChanged(const curUninstallStep: TUninstallStep);
begin
  if curUninstallStep = usPostUninstall then begin
    // try to delete the AppData folder only if confirmed
    if (not UninstallSilent) and (MsgBox('Would you like to keep the mirror tasks and their history?',
      mbConfirmation, MB_YESNO or MB_DEFBUTTON2) = IDNO) then begin
      DelTree(ExpandConstant('{localappdata}\{#MyAppName}'), True, True, True);
    end;
  end;
end;
[Setup]
AppName=Initialize TV Show Child Profiles
AppId=InitializeTVShowChildProfiles
AppVerName=Initialize TV Show Child Profiles 3.0.2.2
AppCopyright=Copyright © Doena Soft. 2010 - 2021
AppPublisher=Doena Soft.
AppPublisherURL=http://doena-journal.net/en/dvd-profiler-tools/
DefaultDirName={commonpf32}\Doena Soft.\Initialize TV Show Child Profiles
; DefaultGroupName=Doena Soft.
DirExistsWarning=No
SourceDir=..\InitializeTVShowChildProfiles\bin\x86\InitializeTVShowChildProfiles
Compression=zip/9
AppMutex=InvelosDVDPro
OutputBaseFilename=InitializeTVShowChildProfilesSetup
OutputDir=..\..\..\..\InitializeTVShowChildProfilesSetup\Setup\InitializeTVShowChildProfiles
MinVersion=0,6.0
PrivilegesRequired=admin
WizardImageFile=compiler:wizmodernimage-is.bmp
WizardSmallImageFile=compiler:wizmodernsmallimage-is.bmp
DisableReadyPage=yes
ShowLanguageDialog=no
VersionInfoCompany=Doena Soft.
VersionInfoCopyright=2010 - 2021
VersionInfoDescription=Initialize TV Show Child Profiles Setup
VersionInfoVersion=3.0.2.2
UninstallDisplayIcon={app}\djdsoft.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Messages]
WinVersionTooLowError=This program requires Windows XP or above to be installed.%n%nWindows 9x, NT and 2000 are not supported.

[Types]
Name: "full"; Description: "Full installation"

[Files]
Source: "djdsoft.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "DVDProfilerHelper.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "DVDProfilerHelper.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "InitializeTVShowChildProfiles.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "InitializeTVShowChildProfiles.pdb"; DestDir: "{app}"; Flags: ignoreversion

Source: "de\DVDProfilerHelper.resources.dll"; DestDir: "{app}\de"; Flags: ignoreversion

; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Run]
Filename: "{win}\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe"; Parameters: "/codebase ""{app}\InitializeTVShowChildProfiles.dll"""; Flags: runhidden

;[UninstallDelete]

[UninstallRun]
Filename: "{win}\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe"; Parameters: "/u ""{app}\InitializeTVShowChildProfiles.dll"""; Flags: runhidden

[Registry]
; Register - Cleanup ahead of time in case the user didn't uninstall the previous version.
Root: HKCR; Subkey: "CLSID\{{B57886BD-AEA0-4a23-A691-2A36175989B5}"; Flags: dontcreatekey deletekey
Root: HKCR; Subkey: "DoenaSoft.DVDProfiler.InitializeTVShowChildProfiles.Plugin"; Flags: dontcreatekey deletekey
Root: HKCU; Subkey: "Software\Invelos Software\DVD Profiler\Plugins\Identified"; ValueType: none; ValueName: "{{B57886BD-AEA0-4a23-A691-2A36175989B5}"; ValueData: "0"; Flags: deletevalue
Root: HKCU; Subkey: "Software\Invelos Software\DVD Profiler_beta\Plugins\Identified"; ValueType: none; ValueName: "{{B57886BD-AEA0-4a23-A691-2A36175989B5}"; ValueData: "0"; Flags: deletevalue
Root: HKLM; Subkey: "Software\Classes\CLSID\{{B57886BD-AEA0-4a23-A691-2A36175989B5}"; Flags: dontcreatekey deletekey
Root: HKLM; Subkey: "Software\Classes\DoenaSoft.DVDProfiler.InitializeTVShowChildProfiles.Plugin"; Flags: dontcreatekey deletekey
; Unregister
Root: HKCR; Subkey: "CLSID\{{B57886BD-AEA0-4a23-A691-2A36175989B5}"; Flags: dontcreatekey uninsdeletekey
Root: HKCR; Subkey: "DoenaSoft.DVDProfiler.InitializeTVShowChildProfiles.Plugin"; Flags: dontcreatekey uninsdeletekey
Root: HKCU; Subkey: "Software\Invelos Software\DVD Profiler\Plugins\Identified"; ValueType: none; ValueName: "{{B57886BD-AEA0-4a23-A691-2A36175989B5}"; ValueData: "0"; Flags: uninsdeletevalue
Root: HKCU; Subkey: "Software\Invelos Software\DVD Profiler_beta\Plugins\Identified"; ValueType: none; ValueName: "{{B57886BD-AEA0-4a23-A691-2A36175989B5}"; ValueData: "0"; Flags: uninsdeletevalue
Root: HKLM; Subkey: "Software\Classes\CLSID\{{B57886BD-AEA0-4a23-A691-2A36175989B5}"; Flags: dontcreatekey uninsdeletekey
Root: HKLM; Subkey: "Software\Classes\DoenaSoft.DVDProfiler.InitializeTVShowChildProfiles.Plugin"; Flags: dontcreatekey uninsdeletekey

[Code]
function IsDotNET45Detected(): boolean;
// Function to detect dotNet framework version 4.0
// Returns true if it is available, false it's not.
var
dotNetStatus: boolean;
begin
dotNetStatus := RegKeyExists(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4');
Result := dotNetStatus;
end;

function InitializeSetup(): Boolean;
// Called at the beginning of the setup package.
begin

if not IsDotNET45Detected then
begin
MsgBox( 'The Microsoft .NET Framework version 4 is not installed. Please install it and try again.', mbInformation, MB_OK );
Result := false;
end
else
Result := true;
end;


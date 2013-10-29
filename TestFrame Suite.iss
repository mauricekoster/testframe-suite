; CGI Nederland BV - copyrights reserved 1990-2013

#define PQM_Version "2008.03.001"
#define TFT_Version "2013.01 Beta"
#define Cluster_Version "2013.1"
#define DEBUG_RELEASE "Debug"

[Setup]
AppName=TestFrame Suite
AppVerName=TestFrame Suite {#DEBUG_RELEASE} {#TFT_Version}
AppVersion={#TFT_Version}
AppPublisher=CGI Nederland BV
DefaultDirName=C:\TestFrame
DisableDirPage=yes
DefaultGroupName=CGI\TestFrame Suite
DisableProgramGroupPage=yes
DisableReadyPage=yes
OutputBaseFilename=TestFrame Suite ({#DEBUG_RELEASE} {#TFT_Version})
Compression=lzma
SolidCompression=yes
; No administrator rights are needed
PrivilegesRequired=none
SetupLogging=yes
InfoAfterFile=readme.txt
UninstallFilesDir={app}\Uninstall_TFSuite

; Branding
WizardImageFile=CGI-LeftSide.bmp
WizardSmallImageFile=cgi-logo.bmp
SetupIconFile=TF.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"

[CustomMessages]
english.ExcelRunning=Excel still running. Please close all Excel programs and restart installer.
dutch.ExcelRunning=Excel is actief. Sluit alstublieft alle Excel programma's en herstart deze installer.

[Files]
; Excel Add-ins. It gets automagical installed
Source: "TestFrameSuiteView\bin\{#DEBUG_RELEASE}\TestFrameSuite.x64-packed.xll"; DestDir: "{app}"; DestName: "TestFrameSuite.x64.xll"; AfterInstall: InstallAddin(ExpandConstant('{app}\TestFrameSuite.x64.xll'), 64); Flags: ignoreversion
Source: "TestFrameSuiteView\bin\{#DEBUG_RELEASE}\TestFrameSuite-packed.xll";     DestDir: "{app}"; DestName: "TestFrameSuite.x86.xll"; AfterInstall: InstallAddin(ExpandConstant('{app}\TestFrameSuite.x86.xll'), 32); Flags: ignoreversion

; Example database
;Source: "Database\Example.mdb"; DestDir: "{app}\Database";

; Help/documentation
Source: "..\Help\TestFrame Suite.chm"; DestDir: "{app}\Documentation";


; -- Templates --
Source: "..\Templates\Product Quality Matrix\PQM cluster (EN) v{#PQM_Version}.xlt"; DestDir: "{app}\Template\Clusters"
Source: "..\Templates\Product Quality Matrix\PQM cluster (NL) v{#PQM_Version}.xlt"; DestDir: "{app}\Template\Clusters"
Source: "..\Clusters\Cluster Workbook - EN - v{#Cluster_Version}.xltx"; DestDir: "{app}\Template\Clusters"
;Source: "..\Clusters\Cluster Workbook - NL - v{#Cluster_Version}.xltx"; DestDir: "{app}\Template\Clusters"


[Icons]
Name: "{group}\TestFrame Suite Help"; Filename: "{app}\Documentation\TestFrame Suite.chm"

[Registry]
Root: HKCU; Subkey: "Software\CGI\TestFrame Suite"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\CGI\TestFrame Suite\General"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\CGI\TestFrame Suite\General"; ValueType: string; ValueName: "TemplatePath"; ValueData: "{app}\Template"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\CGI\TestFrame Suite\General"; ValueType: string; ValueName: "DocumentPath"; ValueData: "{app}\Documentation"; Flags: uninsdeletekey

[Code]
#include "setup_installaddin.inc"

//wpSelectComponents
function InitializeSetup(): Boolean;
var
  Appl: Variant;
begin
    try
      Appl := GetActiveOleObject('Excel.Application');
    except
    end;

    if VarIsEmpty(Appl) then
      Result := True
    else
    begin
      MsgBox(CustomMessage('ExcelRunning') ,mbInformation, MB_OK);
      Result := False
    end;
end;

function InitializeUninstall(): Boolean;
var
  Appl: Variant;
begin
    try
      Appl := GetActiveOleObject('Excel.Application');
    except
    end;

    if VarIsEmpty(Appl) then
      Result := True
    else
    begin
      MsgBox(CustomMessage('ExcelRunning') ,mbInformation, MB_OK);
      Result := False
    end;
end;


procedure CurStepChanged(CurStep: TSetupStep);
begin

  if CurStep = ssInstall then
  begin


      EnableAccessVBOM();

      AddTrustedLocation( 'C:\TestFrame\', 'TestFrame location' );

      // Old TestFrame Toolbars (Excel based)
      
      //UninstallAddin( 'TestFrame Toolbar v2' );
      //UninstallAddin( 'TestFrame Toolbar Ribbon' );
      

      // .NET TestFrame Toolbar
      UninstallAddin( 'TestFrameSuite.x64.xll' );
      UninstallAddin( 'TestFrameSuite.x86.xll' );

  end


end;



procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin

  if  CurUninstallStep = usPostUninstall then
  begin
     // Old TestFrame Toolbars (Excel based)
      //UninstallAddin( 'TestFrame Toolbar v2' );
      //UninstallAddin( 'TestFrame Toolbar Ribbon' );

      // .NET TestFrame Toolbar
      UninstallAddin( 'TestFrameSuite.x64.xll' );
      UninstallAddin( 'TestFrameSuite.x86.xll' );
  end;
end;


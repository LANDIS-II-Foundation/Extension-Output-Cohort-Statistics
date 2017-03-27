#define PackageName      "Output Cohort Statistics"
#define PackageNameLong  "Output Cohort Statistics"
#define Version          "2.1.2"
#define ReleaseType      "official"
#define ReleaseNumber    ""
#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#define ExtDir "C:\Program Files\LANDIS-II\v6\bin\extensions"
#define AppDir "C:\Program Files\LANDIS-II\v6\"
#define LandisPlugInDir "C:\Program Files\LANDIS-II\plug-ins"

#include "package (Setup section) v6.0.iss"


[Files]
; This .dll IS the extension (ie, the extension's assembly)
; NB: Do not put a version number in the file name of this .dll
Source: ..\..\src\bin\debug\Landis.Extension.Output.CohortStats.dll; DestDir: {#ExtDir}; Flags: replacesameversion


; Requisite auxiliary libraries
; NB. These libraries are used by other extensions and thus are never uninstalled.
; There are no auxiliary libraries for this extension.


; Complete example for testing the extension
Source: ..\examples\*.txt; DestDir: {#AppDir}\examples\Output Cohort Stats
Source: ..\examples\*.gis; DestDir: {#AppDir}\examples\Output Cohort Stats
Source: ..\examples\*.bat; DestDir: {#AppDir}\examples\Output Cohort Stats

; User Guides are no longer shipped with installer
;Source: docs\LANDIS-II Age Cohort Statistics v2.1 User Guide.pdf; DestDir: {#AppDir}\docs


; LANDIS-II identifies the extension with the info in this .txt file
; NB. New releases must modify the name of this file and the info in it
#define InfoTxt "Output Cohort Stats 2.1.txt"
Source: {#InfoTxt}; DestDir: {#LandisPlugInDir}


[Run]
;; Run plug-in admin tool to add entries for each plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""Output Cohort Stats"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#InfoTxt}"" "; WorkingDir: {#LandisPlugInDir}

[UninstallRun]
;; Run plug-in admin tool to remove entries for each plug-in

[Code]
{ Check for other prerequisites during the setup initialization }
#include "package (Code section) v3.iss"

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  Result := True
end;
# Name:    BuildAddrNETdll
# Purpose: Compile the mdAddrNET.dll wrapper 

######################### Parameters ##########################

param($license = '', [switch]$quiet = $false)

######################### Classes ##########################

class FileConfig {
  [string] $FileName;
  [string] $ReleaseVersion;
  [string] $OS;
  [string] $Compiler;
  [string] $Architecture;
  [string] $Type;
}

######################### Config ###########################

$RELEASE_VERSION = '2024.01'

# Uses the location of the .ps1 file 
$CurrentPath = $PSScriptRoot
$CmdPath = "C:\Program Files\Microsoft Visual Studio\2022\Professional\VC\Auxiliary\Build\vcvarsall.bat" # Configure the path to vcvarsall.bat if needed

$Wrapper          = [FileConfig]@{
  FileName        = "mdAddr_cSharpCode.cs";
  ReleaseVersion  = $RELEASE_VERSION;
  OS              = "ANY";
  Compiler        = "NET";
  Architecture    = "ANY" ;
  Type            = "INTERFACE"
}

######################## Functions #########################

function DownloadWrapper() {
  Write-Host "MELISSA UPDATER IS DOWNLOADING WRAPPER(S)..."

  # Check for quiet mode
  if ($quiet) {
    .\MelissaUpdater\MelissaUpdater.exe file --filename $Wrapper.FileName --release_version $Wrapper.ReleaseVersion --license $LICENSE --os $Wrapper.OS --compiler $Wrapper.Compiler --architecture $Wrapper.Architecture --type $Wrapper.Type --target_directory $CurrentPath > $null
    if(($?) -eq $False) {
        Write-Host "`nCannot run Melissa Updater. Please check your license key!"
        Exit
    }
  }
  else {
    .\MelissaUpdater\MelissaUpdater.exe file --filename $Wrapper.FileName --release_version $Wrapper.ReleaseVersion --license $LICENSE --os $Wrapper.OS --compiler $Wrapper.Compiler --architecture $Wrapper.Architecture --type $Wrapper.Type --target_directory $CurrentPath 
    if(($?) -eq $False) {
        Write-Host "`nCannot run Melissa Updater. Please check your license key!"
        Exit
    }
  }

  Write-Host "Melissa Updater finished downloading " $Wrapper.FileName "!"
}

########################## Main ############################

Write-Host "`n======================= WELCOME TO MELISSA ADDRESS OBJECT NET WRAPPER =======================`n"

# Get license (either from parameters or user input)
if ([string]::IsNullOrEmpty($license) ) {
  $License = Read-Host "Please enter your license key"
}

# Check for License from Environment Variables 
if ([string]::IsNullOrEmpty($License) ) {
  $License = $env:MD_LICENSE 
}

if ([string]::IsNullOrEmpty($License)) {
  Write-Host "`nLicense Key is invalid!"
  Exit
}

# Download wrapper(s) if Melissa Updater is set up
if (Test-Path (".\MelissaUpdater\MelissaUpdater.exe")) {
  DownloadWrapper -license $License
}

# Start Program
# Build project
Write-Host "`n======================================== BUILD WRAPPER ======================================"

cmd.exe /C """$CmdPath"" x86_x64 && csc -out:mdAddrNET.dll -platform:AnyCPU -target:library mdAddr_cSharpCode.cs" > $null

if (Test-Path ("mdAddrNET.dll")) {
  $filePath = Join-Path -Path (Split-Path -Parent $PSCommandPath) -ChildPath "mdAddrNET.dll"

  Write-Host "`nmdAddrNET.dll has been successfully generated at:`n$filePath`n"
}
else {
  Write-Host "`nError: mdAddrNET.dll was not created successfully`n"
  Write-Host "`nPlease check for the system requirements and necessary files`n"
}


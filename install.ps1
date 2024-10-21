# Downloads and installs: https://github.com/Swagguy47/BigCityCustomKitties and https://github.com/BepInEx/BepInEx
# Usage: 1. Copy this file to the same directory as the game's executable
#        2. Right click this file in explorer and "Run with powershell"

$gameExecutable = './Little Kitty, Big City.exe'

$bepinex_url = 'https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.2/BepInEx_win_x64_5.4.23.2.zip'
$bepinex_zip = './BepInEx_win_x64_5.4.23.2.zip'

$BCCK_url = 'https://github.com/Swagguy47/BigCityCustomKitties/releases/download/v.1.2.0/BCCustomKitties_v1.2.0.zip'
$BCCK_zip = './BCCustomKitties_v1.2.0.zip'

# Error if we're in the wrong dir
if (-not (Test-Path $gameExecutable)) {
  Write-Host "install.ps1 needs to be located next to $gameExecutable, exiting." -ForegroundColor Red
  exit
}

# Prompt about previous installation
if ((Test-Path './Skins') -and ((Read-Host 'Found previous installation, would you like to remove it? [y/n]') -eq 'y')) {
  Remove-Item './Skins' -Force -Recurse -ErrorAction SilentlyContinue
  Remove-Item './.doorstop_version' -Force -Recurse -ErrorAction SilentlyContinue
  Remove-Item './doorstop_config.ini' -Force -Recurse -ErrorAction SilentlyContinue
  Remove-Item './winhttp.dll' -Force -Recurse -ErrorAction SilentlyContinue
}

# Download and install BepinEX 5 LTS
Write-Host "Downloading BepinEx from $bepinex_url" -ForegroundColor Green
Invoke-WebRequest -Uri $bepinex_url -OutFile $bepinex_zip
Expand-Archive -Path $bepinex_zip -DestinationPath './' -Force
Remove-Item $bepinex_zip

# Extracting BigCityCustomKitties
Invoke-WebRequest -Uri $BCCK_url -OutFile $BCCK_zip
Expand-Archive -Path $BCCK_zip -DestinationPath './' -Force
Remove-Item $BCCK_zip

# Backing default skin and replacing with orange
Write-Host 'Testing a custom skin from Skins/Extras' -ForegroundColor Green
Rename-Item -Path './Skins/Current.png' -NewName 'Default.png'
Copy-Item -Path './Skins/extras/Vet.png' -Destination './Skins/Current.png'

Write-Host 'Done!' -ForegroundColor Green
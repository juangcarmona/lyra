# Ensure UTF-8 output (to avoid encoding issues)
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$projectName = "Lyra"
$repoUrl = "https://github.com/juangcarmona/lyra.git"
$installDir = [System.IO.Path]::Combine($env:USERPROFILE, $projectName)
$scriptFolder = [System.IO.Path]::Combine($env:USERPROFILE, "scripts")
$scriptPath = [System.IO.Path]::Combine($scriptFolder, "lyra.cmd")
$repoDir = [System.IO.Path]::Combine($env:TEMP, "lyra_repo")
$projectFile = "$repoDir/src/Lyra/Lyra.csproj"

# Ensure .NET 8 is installed
$dotnetVersion = dotnet --version
if (-not $dotnetVersion) {
    Write-Host "ERROR: .NET SDK is not installed. Install .NET 8 and retry."
    exit 1
}
if ($dotnetVersion -notmatch "^8\..*") {
    Write-Host "ERROR: .NET 8 is required, but found $dotnetVersion. Please upgrade."
    exit 1
}
Write-Host "INFO: .NET 8 detected: $dotnetVersion"

# Ensure installation directory exists
if (-Not (Test-Path -Path $installDir)) {
    New-Item -ItemType Directory -Path $installDir -Force
}

# Clone repository
if (Test-Path -Path $repoDir) {
    Remove-Item -Recurse -Force $repoDir
}
Write-Host "INFO: Cloning repository..."
git clone --depth=1 $repoUrl $repoDir

# Validate project file existence
if (-not (Test-Path $projectFile)) {
    Write-Host "ERROR: LYRA project file not found at expected path: $projectFile"
    Write-Host "INFO: Checking actual repo structure..."
    Get-ChildItem -Path $repoDir -Recurse -Filter "*.csproj"
    exit 1
}

# Build project
Write-Host "INFO: Publishing Lyra..."
dotnet publish $projectFile --configuration Release --output $installDir
if (-not (Test-Path "$installDir/Lyra.dll")) {
    Write-Host "ERROR: Build failed. Lyra.dll not found in $installDir"
    exit 1
}

# Ensure scripts directory exists
if (-Not (Test-Path -Path $scriptFolder)) {
    New-Item -ItemType Directory -Path $scriptFolder -Force
}

# Create launcher script
Write-Host "INFO: Creating launcher script..."
$scriptContent = 'dotnet "%USERPROFILE%\Lyra\Lyra.dll" %*'
$scriptContent | Out-File -FilePath $scriptPath -Encoding ASCII

# Ensure scripts folder is in PATH
$envPath = [System.Environment]::GetEnvironmentVariable("Path", [System.EnvironmentVariableTarget]::User)
if ($envPath -notlike "*$scriptFolder*") {
    [System.Environment]::SetEnvironmentVariable("Path", "$envPath;$scriptFolder", [System.EnvironmentVariableTarget]::User)
    Write-Host "INFO: PATH updated. Restart your terminal for changes to apply."
}

Write-Host "INFO: Installation complete! Use 'lyra' in your terminal to run the program."

# Ensure UTF-8 output (to avoid encoding issues)
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$projectName = "Lyra"
$repoUrl = "https://github.com/juangcarmona/lyra.git"
$installDir = [System.IO.Path]::Combine($env:USERPROFILE, $projectName)
$scriptFolder = [System.IO.Path]::Combine($env:USERPROFILE, "scripts")
$scriptPath = [System.IO.Path]::Combine($scriptFolder, "lyra.cmd")
$repoDir = [System.IO.Path]::Combine($env:TEMP, "lyra_repo")
$projectFile = "$repoDir/src/Lyra/Lyra.csproj"

# Check if Lyra is already installed
if (Test-Path -Path $installDir) {
    Write-Host "WARNING: Lyra is already installed in $installDir. The installation will overwrite existing files." -ForegroundColor Yellow
}

# Ensure .NET SDK is installed
try {
    $dotnetVersion = dotnet --version
} catch {
    $dotnetVersion = $null
}

if (-not $dotnetVersion) {
    Write-Host "ERROR: No .NET SDK detected. Please install .NET 8 from:" -ForegroundColor Red
    Write-Host "https://dotnet.microsoft.com/en-us/download/dotnet/8.0" -ForegroundColor Cyan
    exit 1
}

# Ensure Git is installed
try {
    git --version | Out-Null
} catch {
    Write-Host "ERROR: Git is not installed or not found in PATH. Please install Git from:" -ForegroundColor Red
    Write-Host "https://git-scm.com/downloads" -ForegroundColor Cyan
    exit 1
}

# Ensure .NET version is compatible
if ($dotnetVersion -match "^([8-9]|1[0-9])\..*") {
    Write-Host "INFO: Compatible .NET SDK detected: $dotnetVersion"
} else {
    Write-Host "ERROR: Incompatible .NET SDK version detected: $dotnetVersion" -ForegroundColor Red
    Write-Host "Please install .NET 8 or higher from:" -ForegroundColor Red
    Write-Host "https://dotnet.microsoft.com/en-us/download/dotnet/8.0" -ForegroundColor Cyan
    exit 1
}

# Ensure installation directory exists
if (-Not (Test-Path -Path $installDir)) {
    New-Item -ItemType Directory -Path $installDir -Force | Out-Null
}

# Clean previous temporary repo clone
if (Test-Path -Path $repoDir) {
    Remove-Item -Recurse -Force $repoDir
}

# Clone repository
Write-Host "INFO: Cloning repository..."
git clone --depth=1 $repoUrl $repoDir

# Validate project file existence
if (-not (Test-Path $projectFile)) {
    Write-Host "ERROR: LYRA project file not found at expected path: $projectFile" -ForegroundColor Red
    Write-Host "INFO: Checking actual repo structure..." -ForegroundColor Yellow
    Get-ChildItem -Path $repoDir -Recurse -Filter "*.csproj"
    exit 1
}

# Build project
Write-Host "INFO: Publishing Lyra..."
dotnet publish $projectFile --configuration Release --output $installDir
if (-not (Test-Path "$installDir/Lyra.dll")) {
    Write-Host "ERROR: Build failed. Lyra.dll not found in $installDir" -ForegroundColor Red
    exit 1
}

# Ensure scripts directory exists
if (-Not (Test-Path -Path $scriptFolder)) {
    New-Item -ItemType Directory -Path $scriptFolder -Force | Out-Null
}

# Create launcher script
Write-Host "INFO: Creating launcher script..."
$scriptContent = 'dotnet "%USERPROFILE%\Lyra\Lyra.dll" %*'
$scriptContent | Out-File -FilePath $scriptPath -Encoding ASCII

# Ensure scripts folder is in PATH
$envPathUser = [System.Environment]::GetEnvironmentVariable("Path", [System.EnvironmentVariableTarget]::User)
if ($envPathUser -notlike "*$scriptFolder*") {
    # Update user's PATH permanently
    [System.Environment]::SetEnvironmentVariable("Path", "$envPathUser;$scriptFolder", [System.EnvironmentVariableTarget]::User)
    Write-Host "INFO: PATH updated in environment variables." -ForegroundColor Yellow

    # Update PATH in current session
    $env:Path += ";$scriptFolder"
    Write-Host "INFO: PATH updated in the current session." -ForegroundColor Yellow
}


Write-Host "INFO: Installation complete! Use 'lyra' in your terminal to run the program." -ForegroundColor Green

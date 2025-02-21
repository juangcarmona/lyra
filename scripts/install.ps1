$projectName = "Lyra"
$repoUrl = "https://github.com/juangcarmona/lyra.git"
$installDir = [System.IO.Path]::Combine($env:USERPROFILE, $projectName)
$scriptFolder = [System.IO.Path]::Combine($env:USERPROFILE, "scripts")
$scriptPath = [System.IO.Path]::Combine($scriptFolder, "lyra.cmd")

# Ensure .NET 8 is installed
$dotnetVersion = dotnet --version
if (-not $dotnetVersion) {
    Write-Host "❌ .NET SDK is not installed. Please install .NET 8 and retry."
    exit 1
}
if ($dotnetVersion -notmatch "^8\..*") {
    Write-Host "❌ .NET 8 is required, but found $dotnetVersion. Please upgrade."
    exit 1
}

Write-Host "✅ .NET 8 detected: $dotnetVersion"

# Ensure installation directory exists
if (-Not (Test-Path -Path $installDir)) {
    New-Item -ItemType Directory -Path $installDir -Force
}

# Clone the repository
$repoDir = [System.IO.Path]::Combine($env:TEMP, "lyra_repo")
if (Test-Path -Path $repoDir) {
    Remove-Item -Recurse -Force $repoDir
}

Write-Host "📥 Cloning repository..."
git clone $repoUrl $repoDir

# Ensure the correct project path
if (-not (Test-Path "$repoDir/src/Lyra/Lyra.csproj")) {
    Write-Host "❌ Error: LYRA project file not found. Please check the repository structure."
    exit 1
}

# Publish the project
Write-Host "📦 Publishing $projectName..."
dotnet publish "$repoDir/src/Lyra/Lyra.csproj" --configuration Release --output $installDir

# Ensure the scripts directory exists
if (-Not (Test-Path -Path "$env:USERPROFILE\scripts")) {
    New-Item -ItemType Directory -Path $scriptFolder -Force
}

# Create launcher script
Write-Host "🚀 Creating executable wrapper..."
$scriptContent = 'dotnet "%USERPROFILE%\Lyra\Lyra.dll" %*'
$scriptContent | Out-File -FilePath $scriptPath -Encoding ASCII

# Ensure the script folder is in the PATH
$envPath = [System.Environment]::GetEnvironmentVariable("Path", [System.EnvironmentVariableTarget]::User)
if ($envPath -notlike "*$env:USERPROFILE\scripts*") {
    [System.Environment]::SetEnvironmentVariable("Path", "$envPath;$env:USERPROFILE\scripts", [System.EnvironmentVariableTarget]::User)
}

Write-Host "🎉 Installation complete! Use lyra in your terminal to see available commands."

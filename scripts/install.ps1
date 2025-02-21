$projectName = "Lyra"
$repoUrl = "https://github.com/juangcarmona/lyra.git"
$installDir = Join-Path $env:USERPROFILE $projectName
$scriptFolder = Join-Path $env:USERPROFILE "scripts"
$scriptPath = Join-Path $scriptFolder "lyra.cmd"

# Check if .NET 8 is installed
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

# Download source code
$repoDir = Join-Path $env:TEMP "lyra_repo"
if (Test-Path -Path $repoDir) {
    Remove-Item -Recurse -Force $repoDir
}

Write-Host "📥 Cloning repository..."
git clone $repoUrl $repoDir

# Ensure correct project path
if (-not (Test-Path "$repoDir/src/Lyra/Lyra.csproj")) {
    Write-Host "❌ Error: LYRA project file not found. Please check the repository structure."
    exit 1
}

# Publish in Release mode
Write-Host "📦 Publishing $projectName..."
dotnet publish "$repoDir/src/Lyra/Lyra.csproj" --configuration Release --output $installDir

# Ensure the scripts directory exists
if (-not (Test-Path $scriptFolder)) {
    New-Item -ItemType Directory -Path $scriptFolder | Out-Null
}

# Create launcher script
Write-Host "🚀 Creating executable wrapper..."
$scriptContent = "dotnet `"$installDir\Lyra.dll`" `$args"
Out-File -FilePath $scriptPath -Encoding ASCII -InputObject $scriptContent

# Add script folder to user's PATH if not already present
$envPath = [System.Environment]::GetEnvironmentVariable("Path", "User") -split ";"  
if (-not ($envPath -contains $scriptFolder)) {
    $newPath = ($envPath + $scriptFolder) -join ";"  
    [System.Environment]::SetEnvironmentVariable("Path", $newPath, "User")  
    Write-Host "🔧 PATH updated! Restart your terminal to apply changes."
}



# Write-Host "🎉 Installation complete! Use '"lyra'" in your terminal to see all available commands."

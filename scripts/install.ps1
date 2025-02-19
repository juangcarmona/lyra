$projectName = "Lyra"
$installDir = Join-Path $env:USERPROFILE $projectName
$scriptFolder = Join-Path $env:USERPROFILE "scripts"
$scriptPath = Join-Path $scriptFolder "lyra.cmd"

# Check if .NET 8 is installed
$dotnetVersion = dotnet --version
if (-not $dotnetVersion) {
    Write-Host "❌ .NET SDK is not installed. Please install .NET 8 and retry." -ForegroundColor Red
    exit 1
}
if ($dotnetVersion -notmatch "^8\..*") {
    Write-Host "❌ .NET 8 is required, but found $dotnetVersion. Please upgrade." -ForegroundColor Red
    exit 1
}

Write-Host "✅ .NET 8 detected: $dotnetVersion" -ForegroundColor Green

# Create install directory
if (-not (Test-Path $installDir)) {
    New-Item -ItemType Directory -Path $installDir | Out-Null
}

# Publish in Release mode
Write-Host "📦 Publishing $projectName..."
dotnet publish "../src/$projectName/$projectName.csproj" --configuration Release --output $installDir

# Ensure the scripts directory exists
if (-not (Test-Path $scriptFolder)) {
    New-Item -ItemType Directory -Path $scriptFolder | Out-Null
}

# Create launcher script
Write-Host "🚀 Creating executable wrapper..."
$scriptContent = "dotnet `"$installDir\$projectName.dll`" `$args"
Out-File -FilePath $scriptPath -Encoding ASCII -InputObject $scriptContent

# Add script folder to user's PATH if not already present
$envPath = [System.Environment]::GetEnvironmentVariable("Path", "User")
if ($envPath -notlike "*$scriptFolder*") {
    [System.Environment]::SetEnvironmentVariable("Path", "$envPath;$scriptFolder", "User")
    Write-Host "🔧 PATH updated! Restart your terminal to apply changes." -ForegroundColor Yellow
}

Write-Host "🎉 Installation complete! Use 'lyra <URL>' to download MP3 from YouTube." -ForegroundColor Green

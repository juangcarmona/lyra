$projectName = "Lyra"
$installDir = Join-Path $env:USERPROFILE $projectName

if (-not (Test-Path $installDir)) {
    New-Item -ItemType Directory -Path $installDir | Out-Null
}

dotnet publish "../src/$projectName/$projectName.csproj" --configuration Release --output $installDir

$scriptContent = "dotnet `"$installDir\$projectName.dll`" `$args"
$scriptFolder = Join-Path $env:USERPROFILE "scripts"
if (-not (Test-Path $scriptFolder)) {
    New-Item -ItemType Directory -Path $scriptFolder | Out-Null
}

$scriptPath = Join-Path $scriptFolder "lyra.cmd"
Out-File -FilePath $scriptPath -Encoding ASCII -InputObject $scriptContent

# Añadir 'scripts' al Path del usuario si no está
$envPath = [System.Environment]::GetEnvironmentVariable("Path", "User")
if ($envPath -notlike "*$scriptFolder*") {
    [System.Environment]::SetEnvironmentVariable("Path", "$envPath;$scriptFolder", "User")
}

Write-Host "Instalación completada. Reinicia tu terminal para usar 'lyra <URL>' como comando global."

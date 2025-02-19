#!/bin/bash

set -e  # Stop execution on any error

PROJECT_NAME="Lyra"
INSTALL_DIR="/usr/local/share/$PROJECT_NAME"
BIN_DIR="/usr/local/bin"
EXECUTABLE="$BIN_DIR/lyra"

# Check if .NET 8 is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK is not installed. Please install .NET 8 and retry."
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
if [[ ! $DOTNET_VERSION =~ ^8\. ]]; then
    echo "❌ .NET 8 is required, but found $DOTNET_VERSION. Please upgrade."
    exit 1
fi

echo "✅ .NET 8 detected: $DOTNET_VERSION"

# Create install directory
sudo mkdir -p "$INSTALL_DIR"

# Publish in Release mode
echo "📦 Publishing $PROJECT_NAME..."
dotnet publish "../src/$PROJECT_NAME/$PROJECT_NAME.csproj" --configuration Release --output "$INSTALL_DIR"

# Create a launcher script
echo "🚀 Creating executable wrapper..."
echo "#!/bin/bash
dotnet $INSTALL_DIR/$PROJECT_NAME.dll \"\$@\"" | sudo tee "$EXECUTABLE" > /dev/null

# Make the launcher executable
sudo chmod +x "$EXECUTABLE"

echo "🎉 Installation complete! Use 'lyra <URL>' to download MP3 from YouTube."

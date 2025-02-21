#!/bin/bash

set -e  # Stop execution on any error

# Ensure sudo is used for required operations
echo "🔑 This script requires sudo privileges to install LYRA."
sudo -v

PROJECT_NAME="LYRA"
REPO_URL="https://github.com/juangcarmona/lyra.git"
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

# Clone the repository
echo "📥 Cloning repository..."
TEMP_DIR=$(mktemp -d)
git clone $REPO_URL "$TEMP_DIR"

# Ensure correct project path
if [ ! -f "$TEMP_DIR/src/Lyra/Lyra.csproj" ]; then
    echo "❌ Error: LYRA project file not found. Please check the repository structure."
    exit 1
fi

# Create install directory with correct permissions
sudo mkdir -p "$INSTALL_DIR"
sudo chmod -R 777 "$INSTALL_DIR"

# Publish in Release mode
echo "📦 Publishing $PROJECT_NAME..."
sudo dotnet publish "$TEMP_DIR/src/Lyra/Lyra.csproj" --configuration Release --output "$INSTALL_DIR"

# Ensure correct permissions after publish
sudo chmod -R 755 "$INSTALL_DIR"

# Create a launcher script
echo "🚀 Creating executable wrapper..."
echo "#!/bin/bash
dotnet $INSTALL_DIR/Lyra.dll \"\$@\"" | sudo tee "$EXECUTABLE" > /dev/null

# Make the launcher executable
sudo chmod +x "$EXECUTABLE"

# Clean up
echo "🧹 Cleaning up..."
sudo rm -rf "$TEMP_DIR"

echo "🎉 Installation complete! Use 'lyra' to see all available commands."

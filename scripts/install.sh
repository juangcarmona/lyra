
#!/bin/bash

PROJECT_NAME="Lyra"
INSTALL_DIR="/usr/local/share/$PROJECT_NAME"

sudo mkdir -p "$INSTALL_DIR"

# Publicar en modo Release
dotnet publish "../src/$PROJECT_NAME/$PROJECT_NAME.csproj" --configuration Release --output "$INSTALL_DIR"

# Crear un script en /usr/local/bin que invoque la dll
echo "#!/bin/bash
dotnet $INSTALL_DIR/$PROJECT_NAME.dll \"\$@\"" | sudo tee /usr/local/bin/lyra > /dev/null

sudo chmod +x /usr/local/bin/lyra

echo "Instalación completada. Usa 'lyra <URL>' para descargar MP3 desde YouTube."

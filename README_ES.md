# 𝐋𝐘𝐑𝐀 - 𝐋ightweight 𝐘outube 𝐑ipping 𝐀ssistant

![LYRA Mosaic](assets/lyra.webp)

##Introducción

𝐋𝐘𝐑𝐀 (𝐋ightweight 𝐘outube 𝐑ipping 𝐀ssistant) es una herramienta de línea de comandos moderna y eficiente diseñada para extraer y descargar audio de videos y listas de reproducción de YouTube. Está inspirada en un proyecto anterior, [**YoutubeMp3Downloader**](https://github.com/juangcarmona/youtube-mp3-downloader), que fue un intento temprano de crear una herramienta similar. Si bien ese proyecto tenía sus puntos fuertes, LYRA va más allá al ofrecer una experiencia de CLI optimizada, un rendimiento mejorado y compatibilidad con listas de reproducción.

##Características
- **Descarga un solo video de YouTube como MP3**.
- **Descarga todos los archivos de audio de una lista de reproducción**.
- **Rápido, liviano y fácil de usar**.
- **Multiplataforma**: funciona en Linux, macOS y Windows.
- **Integración con FFmpeg** para conversión de audio de alta calidad.

## Instalación

### Linux/macOS
```bash
curl -sSL https://raw.githubusercontent.com/juangcarmona/lyra/main/scripts/install.sh | bash
```

### Windows (PowerShell)
``powershell
iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/juangcarmona/lyra/main/scripts/install.ps1'))
```

## Uso

### Descargar un solo video de YouTube como MP3
```bash
lyra https://www.youtube.com/watch?v=YOUR_VIDEO_ID
```

### Descargar todos los archivos de audio de una lista de reproducción
```bash
lyra -p https://www.youtube.com/playlist?list=YOUR_PLAYLIST_ID
```

## Hoja de ruta
- [ ] Admitir descargas de un solo video
- [ ] Implementar descargas de listas de reproducción
- [ ] Agregar descargas multiproceso
- [ ] Mejorar el manejo de metadatos (etiquetas ID3, carátula del álbum)
- [ ] Proporcionar versiones binarias para una fácil instalación

## Contribuciones
¡Las contribuciones son bienvenidas! No dudes en enviar problemas, solicitudes de funciones o solicitudes de incorporación de cambios para mejorar LYRA.

##Licencia
LYRA tiene licencia Apache 2.0. Consulta el archivo [LICENSE](LICENSE) para obtener más detalles.

---

**LYRA: extrae música de la web sin esfuerzo.** 🎶
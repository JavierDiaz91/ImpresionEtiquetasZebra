# ImpresionEtiquetasZebra

Este proyecto es una aplicación de escritorio desarrollada en **C# (Windows Forms)** diseñada para la **impresión de etiquetas en Impresoras Zebras o cualquier marca que soporte el lenguage ZPL**.

Permite a los usuarios cargar datos desde un archivo **Excel** para generar y enviar etiquetas personalizadas a impresoras térmicas Zebra compatibles. 
Las etiquetas incluyen información como el menú, fechas de elaboración y vencimiento, nombre del empleado y un código de barras EAN-13.

## Características principales:

* **Carga de datos desde Excel:** Facilita la importación de información para la impresión masiva de etiquetas.
* **Generación de ZPL:** Convierte los datos del Excel en comandos ZPL específicos para impresoras Zebra o cualquier marca que soporte ZPL.
* **Selección de impresora:** Permite elegir entre las impresoras instaladas en el sistema.
* **Personalización de etiquetas:** Incluye campos como nombre de empleado, menú, fechas y código de barras.

## 🚀 Levantar el proyecto localmente (desde cero)

### Requisitos
- Windows 10/11 (64-bit recomendado)  
- **Visual Studio 2022** con el workload *Desktop development with .NET*  
- **Microsoft Excel** instalado (necesario para `Microsoft.Office.Interop.Excel`)  
- Impresora Zebra/compatible ZPL (opcional para pruebas reales)

### Pasos
1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/<tu-usuario>/<repo>.git
   cd <repo>
   ```
2. **Abrir la solución en Visual Studio**  
   Abrí el archivo `.sln` que está en la raíz *(por ej., `ImpresionGPC.sln`)*.

3. **Seleccionar plataforma según tu Office**  
   - Office **32-bit** → Plataforma **x86**  
   - Office **64-bit** → Plataforma **x64**  
   *(Menú desplegable de VS: `Any CPU` → elegí `x86` o `x64`)*

4. **Restaurar y compilar**  
   - Visual Studio restaura paquetes automáticamente al abrir.  
   - Menú **Build → Build Solution** (o `Ctrl+Shift+B`).

5. **Establecer proyecto de inicio y ejecutar**  
   - Asegurate de que el proyecto de UI esté como *StartUp Project* (clic derecho → **Set as StartUp Project**).  
   - **F5** para ejecutar.

> Si aparece un error relacionado con **Interop Excel**, revisá que la **plataforma** (x86/x64) del proyecto coincida con la **arquitectura de Office** instalada.

### (Opcional) Compilar por línea de comandos
```powershell
msbuild .\ImpresionGPC\ImpresionGPC.csproj /p:Configuration=Release /p:Platform="x64"
```
# 📦 SICAVI
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![WinUI](https://img.shields.io/badge/WinUI-3-blue)
![SQLite](https://img.shields.io/badge/Database-SQLite-green)
![Status](https://img.shields.io/badge/status-en%20desarrollo-yellow)

**Sistema de Inventario, Compras y Ventas**  
Aplicación de escritorio moderna construida con WinUI 3 y arquitectura MVVM.

![Sicavi Logo](sicavi-logo.png)

Guía de configuración y desarrollo — v1.0

---

## 📌 1. Descripción

SICAVI es una aplicación diseñada para gestionar:

- 📦 Inventario  
- 💰 Ventas  
- 🧾 Facturación  
- 📊 (Próximamente) Dashboard y analítica  
- 🤖 (Futuro) Integración con IA  

### 🛠 Stack tecnológico

| Tecnología | Uso |
|----------|------|
| WinUI 3 | Interfaz de usuario |
| .NET 8 | Backend |
| SQLite | Base de datos |
| Entity Framework Core | ORM |
| CommunityToolkit.Mvvm | MVVM |
| DataGrid Toolkit | UI avanzada |

---

## ⚙️ Requisitos

- Visual Studio 2022 (17.8+)
- .NET 8 SDK
- Windows App SDK
- Git

### Workloads necesarios

Instalar en Visual Studio:

- Desarrollo de escritorio con .NET  
- Desarrollo de aplicaciones de Windows (WinUI)

### ✔ Verificación

```bash
dotnet --version
```

---

## 🚀 Instalación

### 1. Clonar repositorio

```bash
git clone https://github.com/Fra46/SICAVI.git
cd SICAVI
```

### 2. Abrir proyecto
- Abrir SICAVI.sln en Visual Studio
- Esperar restauración de paquetes

### 3. Ejecutar

```bash
F5
```

---

## 🗄 Base de datos
- Tipo: SQLite
- Creación automática al ejecutar

### 📍 Ruta:

```bash
C:\Users\TU_USUARIO\AppData\Local\SICAVI\sicavi.db
```

### 🔄 Migraciones

Crear migración:

```bash
cd SICAVI.DAL
dotnet ef migrations add NombreDescriptivo --startup-project ../SICAVI
```

Eliminar última migración:

```bash
dotnet ef migrations remove --startup-project ../SICAVI
```

Actualizar BD:

```bash
dotnet ef database update NombreMigracion --startup-project ../SICAVI
```

---

## 🧱 Estructura del proyecto

```bash
SICAVI/
 ├── Views/           # UI (XAML)
 ├── ViewModels/      # Lógica MVVM
 ├── Converters/      # Binding helpers
 ├── SICAVI.DAL/      # Acceso a datos
 │   ├── Models/
 │   ├── Data/
 │   ├── Services/
 │   └── Migrations/
 ```

---

## 📏 Convenciones de código

### Modelos

```bash
[ObservableProperty]
private string nombre;
```

### Commands

```bash
[RelayCommand]
private void EliminarProducto() { }
```

### Servicios

```bash
DalExecutor.Execute(() => _context.Productos.ToList());
```

### Manejo de errores

```bash
catch (Exception ex)
{
    MensajeError = ex.Message;
    HayError = true;
}
```

### Convenciones

| Elemento | Formato         |
| -------- | --------------- |
| Clases   | PascalCase      |
| Métodos  | PascalCase      |
| Campos   | _camelCase      |
| XAML     | NombreView.xaml |


---

### ▶️ Ejecución y Debug
- ▶ Ejecutar: F5
- 🐞 Breakpoints: F9
- 📊 Logs: Ventana Output (EF Core)

### 🐞 Depuración
- Breakpoints (F9)
- Output → logs SQL
- Event Viewer si falla

---

## ⚠️ Errores comunes

| Error                  | Solución             |
| ---------------------- | -------------------- |
| XamlParseException     | Revisar recursos     |
| SqliteException        | Reintentar ejecución |
| DbUpdateException      | Validar datos        |
| NullReferenceException | Revisar Include()    |

---

## 📊 Fases pendientes

### 📈 Fase 4 — Dashboard
- Ventas del día/mes
- Stock crítico
- Últimas ventas
- Gráfica semanal

### 📁 Archivos:

```bash
Views/DashboardView.xaml  
ViewModels/DashboardViewModel.cs
```

### 📤 Reportes

Instalar:

### Excel

```bash
dotnet add package ClosedXML
```

### PDF

```bash
dotnet add package QuestPDF
```

---

## 🔐 Fase 5 — Configuración y Login

### Configuración
- Nombre tienda
- IVA
- SMTP
- Impresora

### Login
- Validación desde SQLite
- Roles: Admin / Empleado

### Seguridad

```bash
dotnet add package BCrypt.Net-Next
```

Uso:

```bash
BCrypt.HashPassword(password);
BCrypt.Verify(password, hash);
```

---

## 🌿 Git Workflow

### Crear rama

```bash
git checkout -b feature/nueva-feature
```

### Commit

```bash
git commit -m "feat: nueva funcionalidad"
```

---

## 👨‍💻 Autores

Andres Zapata
- GitHub: [@Fra46](https://github.com/Fra46)

Maira Torres
- GitHub: [@22MAT11](https://github.com/22MAT11)

---

SICAVI — Documentación interna v1.0 — 2025

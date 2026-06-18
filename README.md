# Skvia Architecture Template (skvia-api)

¡Bienvenido a la plantilla oficial de arquitectura base de **Skvia**! Este repositorio sirve como el molde genérico e industrializado de nuestra empresa para la creación de proyectos y microservicios basados en **.NET 10**, estructurados bajo principios de **Diseño Guiado por el Dominio (DDD)** y **Vertical Slice Architecture**.

Esta plantilla automatiza la tediosa tarea de configuración inicial (DbContext, Seguridad JWT, Validaciones Fluidas, Sembrado dinámico, etc.), permitiéndote desplegar una API con calidad de producción en un solo segundo mediante la CLI de .NET.

---

## 🚀 Características Clave de la Arquitectura

- **Minimal APIs:** Implementación ligera, moderna y de alto rendimiento utilizando el ecosistema nativo de .NET.
- **Vertical Slice Architecture:** En lugar de dividir el código por capas técnicas horizontales difusas (Controllers/Services), cada funcionalidad (Slice) vive de forma cohesiva dentro de su propia carpeta de características (`Features/`).
- **Domain-Driven Design (DDD):** Entidades de dominio ricas con encapsulamiento estricto, métodos mutadores semánticos y tipos encapsulados utilizando el soporte nativo completo para la palabra clave `field` de C#.
- **Auditoría Centralizada (Odoo Style):** Sistema de activación/desactivación lógica (`IsActive`) mediante los métodos universales `.Archive()` y `.Unarchive()` heredados de la clase base abstracta `BaseAuditableEntity`.
- **Filtros de Consulta Globales:** Ocultamiento automático de entidades archivadas a nivel de Entity Framework Core (`HasQueryFilter`), controlable en consultas críticas con `.IgnoreQueryFilters()`.
- **Sembrado Dinámico por Reflexión:** Sistema automático que descubre permisos constantes en C#, extrae sus etiquetas legibles desde los atributos `[Description]` y los inserta en la base de datos de manera incremental y segura.

---

## 🛠️ Requisitos Previos

Asegúrate de tener instalado en tu entorno local:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) o superior.
- Una herramienta de terminal (PowerShell, CMD, Bash o la integrada en tu IDE).

---

## 📦 Instalación de la Plantilla

Para registrar esta plantilla en el motor global de tu sistema .NET, sigue estos pasos:

1. Abre una terminal dentro de la carpeta raíz de este molde genérico (donde se encuentra el archivo `.template.config`).
2. Limpia cualquier registro o rastro previo ejecutando:

   ```bash
   dotnet new uninstall .

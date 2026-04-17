# TaskManager API

REST API para gestión de tareas y proyectos en equipo, construida con ASP.NET Core 8.

## 🚀 Demo en vivo

[Swagger UI](https://task-manager-api-production-4d30.up.railway.app/swagger/index.html) — probá la API directamente desde el navegador sin instalar nada.

## ¿Qué hace?

Sistema tipo Jira simplificado: los usuarios se registran, crean proyectos, agregan miembros al equipo, crean tareas con estados y las asignan a miembros. Incluye paginación y filtros en los listados.

## Stack técnico

- **ASP.NET Core 8** — framework principal
- **Entity Framework Core** — ORM con migraciones
- **PostgreSQL** — base de datos
- **JWT** — autenticación
- **xUnit + InMemory** — unit tests
- **Swagger / OpenAPI** — documentación interactiva
- **Railway** — deploy

## Endpoints principales

### Auth
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | /api/auth/register | Registrar usuario |
| POST | /api/auth/login | Login |

### Proyectos
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | /api/proyectos | Listar proyectos (paginado, filtro por nombre) |
| GET | /api/proyectos/{id} | Detalle con miembros |
| POST | /api/proyectos | Crear proyecto |
| PUT | /api/proyectos/{id} | Actualizar (solo propietario) |
| DELETE | /api/proyectos/{id} | Eliminar (solo propietario) |
| POST | /api/proyectos/{id}/members | Agregar miembro |

### Tareas
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | /api/proyectos/{id}/tareas | Listar tareas (paginado, filtro por estado) |
| GET | /api/proyectos/{id}/tareas/{tareaId} | Detalle de tarea |
| POST | /api/proyectos/{id}/tareas | Crear tarea |
| PUT | /api/proyectos/{id}/tareas/{tareaId} | Actualizar tarea |
| PATCH | /api/proyectos/{id}/tareas/{tareaId}/estado | Cambiar estado |
| DELETE | /api/proyectos/{id}/tareas/{tareaId} | Eliminar tarea |

## Correrlo localmente

### Requisitos
- .NET 8 SDK
- PostgreSQL

### Pasos

1. Cloná el repo (git clone https://github.com/tu-usuario/TaskManagerAPI.git)
2. Configurá `appsettings.Development.json` en `TaskManagerAPI/TaskManagerAPI/`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=TaskManagerDb;Username=postgres;Password=TU_PASSWORD"
  },
  "JwtSettings": {
    "Secret": "una-clave-secreta-larga-minimo-32-caracteres",
    "ExpirationHours": 24
  }
}
```

3. Aplicá las migraciones: dotnet ef database update --project TaskManagerAPI
4. Levantá el proyecto: dotnet run --project TaskManagerAPI
5. Abrí Swagger en `https://localhost:{puerto}/swagger`

## Arquitectura
Controllers     → reciben el request y delegan
Services        → lógica de negocio
DTOs            → contratos de entrada y salida
Models          → entidades de base de datos
Middleware      → manejo global de excepciones
Tests           → unit tests con xUnit e InMemory

## Tests
dotnet test
9 unit tests cubriendo la capa de servicios — creación, permisos, filtros y validaciones.

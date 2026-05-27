# SPEC 01 : Setup inicial

## Análisis funcional

Quiero crear un proyecto para manejar el cálculo de participaciones de empresas sobre otras para generar reportes específicos para una entidad reguladora de un país determinado.
Dentro de la aplicación tenemos las algunas entidades.
El algoritmo de cálculo y el formato de los reportes se verá en una siguiente `spec`.

### Entidades

#### Persona

- Nombre o Razón social
- Tipo de persona (mirar **Tipo de persona**)
- CIIU (Clasificación Industrial Internacional Uniforme; es un texto de longitud fija, aunque la longitud exacta se definirá más adelante)
- Dirección (donde está ubicada la persona)
- Tipo de documento (una `Persona` tiene un único `TipoDocumento`; los tipos disponibles dependen del `TipoPersona` — ver **Tipo de documento**)
- Número de documento (su formato se valida con la expresión regular del `TipoDocumento` seleccionado, si tiene una)
- Código entidad (es un código asignado por la entidad reguladora; puede ser nulo — cualquier regla de negocio sobre él se aplica a nivel de validación en la aplicación)
- Representante (funcionario dentro de la empresa, que tiene la entidad `Persona`; obligatorio para `PersonaJurídica` y `EnteJurídico`; vacío para `PersonaNatural`)
- Flag Reporte (boolean, que indica si se incluye como raíz del cálculo de acciones, se explicará en una siguiente `spec`)
- País (entidad `Pais`)
- Ubicación interna en el país (para la base de datos es un texto libre, pero la aplicación puede aplicarle una regla, que se explicará en una siguiente `spec`)

#### Tipo de persona

Se trata de un enumerable de la aplicación:

- Persona natural
- Persona jurídica
- Ente jurídico

#### Tipo de documento

Cada `TipoDocumento` está disponible para uno o más `TipoPersona`. Una `Persona` solo puede tener un `TipoDocumento`, y este debe ser válido para su `TipoPersona` (ej., un DNI aplica a `PersonaNatural` pero no a `PersonaJurídica`).

La relación entre `TipoDocumento` y `TipoPersona` se almacena en una tabla de mapeo, donde el valor del enum `TipoPersona` se guarda como `int`.

- Nombre (ej., Documento Nacional de Identidad)
- Abreviatura (ej., DNI)
- Validación (expresión regular, puede ser nula)
- Tipos de persona habilitados (tabla de mapeo: `DocumentTypePersonType(DocumentTypeId, PersonType int)`)

#### País

- Nombre
- Abreviatura (Estados Unidos: US, Reino Unido: UK)

#### Participación

- Sociedad (tiene la entidad `Persona`)
- Accionista (tiene la entidad `Persona`)
- Porcentaje de participación (0% a 100%)
- Fecha inicio de vigencia (indica desde cuándo aplica)
- Fecha fin de vigencia (puede no tener si es vigente o indicar hasta cuándo aplica)

#### Cargo

Catálogo compartido usado por `Junta directiva` (cargo principal y secundario). Incluye un ítem especial `Sin cargo` para cuando no aplica un cargo secundario.

- Nombre

### Junta directiva

- Sociedad (entidad `Persona`)
- Integrante (entidad `Persona`, tipo de persona: `Persona natural`)
- Cargo principal (entidad `Cargo`)
- Cargo secundario (entidad `Cargo`; no nulo — usar ítem `Sin cargo` cuando no aplica)

## Análisis técnico

### Descripción

El proyecto está dentro de un mismo repositorio.

### Frontend

> **Regla de proyecto:** Nunca usar `npm` para instalar dependencias ni ejecutar scripts. Usar exclusivamente `pnpm`. Esto aplica a todos los comandos (`pnpm install`, `pnpm add`, `pnpm run`, etc.). Motivo: vulnerabilidad de seguridad identificada en `npm`.

- Ya he creado el proyecto base en la carpeta `/app`
- Debe tener un sidebar en el lado izquierdo colapsable: cuando está colapsado se apreciarán solo los íconos de las opciones, y persistirá en `localStorage` vía Zustand
- Debe tener theme light/dark con persistencia en `localStorage` vía Zustand

#### Rutas

| Ruta              | Módulo                                   |
| ----------------- | ---------------------------------------- |
| `/persons`        | Personas                                 |
| `/person-types`   | Tipos de persona (solo lectura, es enum) |
| `/document-types` | Tipos de documento                       |
| `/countries`      | Países                                   |
| `/participations` | Participaciones                          |
| `/board`          | Junta directiva                          |
| `/positions`      | Cargos                                   |
| `/users`          | Usuarios                                 |

#### Stack

- pnpm (nunca con npm)
- React
- Vite
- TypeScript
- Zustand
- Tanstack Query
- Shadcn / Tweakcn
- Tailwind
- Lucide-React

### Backend

#### Descripción

- El proyecto tendra Clean Architecture
- Ya he creado la solución con sus respectivos proyectos en la carpeta `/api`
- La solución tendrá un único `Context`, que incluirá las entidades del negocio, así como las de AuthZ/AuthN

#### Seguridad

- Se trabajará con las entidades de Microsoft (AspNetUsers, AspNetRoles, etc.)
- Se trabajará con JWT, Access tokens y Refresh tokens
- Roles mínimos:
  - `Admin`: acceso total (`Permission.Admin`)
  - `Guest`: solo lectura (`PersonRead | ParticipationRead | BoardRead | ReportRead`)

#### Permisos (Bitwise Flags)

Los permisos se modelan como un `[Flags] enum` en C#, almacenados como `int` en base de datos. Cada rol tendrá asignado un valor combinado de permisos.

```csharp
[Flags]
public enum Permission
{
    None          = 0,

    // Persons
    PersonRead    = 1 << 0,   // 1
    PersonWrite   = 1 << 1,   // 2
    PersonDelete  = 1 << 2,   // 4

    // Participations
    ParticipationRead   = 1 << 3,   // 8
    ParticipationWrite  = 1 << 4,   // 16
    ParticipationDelete = 1 << 5,   // 32

    // Board of directors
    BoardRead   = 1 << 6,   // 64
    BoardWrite  = 1 << 7,   // 128
    BoardDelete = 1 << 8,   // 256

    // Reports
    ReportRead     = 1 << 9,    // 512
    ReportGenerate = 1 << 10,   // 1024

    // Users
    UserRead   = 1 << 11,   // 2048
    UserWrite  = 1 << 12,   // 4096
    UserDelete = 1 << 13,   // 8192

    Admin = ~0
}
```

Verificación:

```csharp
bool canWrite = (user.Permissions & Permission.PersonWrite) != 0;
```

#### Stack

- .NET 10
- C#
- MediatR (uso de Queries, Commands, Handlers, Validators automáticos)
- FluentValidation (FluentValidation.DependencyInjectionExtensions)
- CQRS
- EF Core (versión compatible y estable más reciente)
- AutoMapper
- Microsoft Identity
- Directory.Packages

### Base de datos

- PostgreSQL (local: Docker; nube: puede ser Azure o Supabase)

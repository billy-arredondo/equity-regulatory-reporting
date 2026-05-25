# SPEC 01 : Setup inicial

## Análisis funcional

Quiero crea un proyecto para manejar el cálculo de participaciones de empresas sobre otras para generar reportes específicos para una entidad reguladora de un país determinado.
Dentro de la aplicación tenemos las siguientes entidades:

### Entidades

#### Empresa

- Nombre o Razón social
- Tipo de persona (Persona Natural, Persona Jurídica o Ente Jurídico)
- CIIU (código de actividad de la empresa)
- Dirección
- Tipo de documento (depende del tipo de persona, TODO: se indica más adelante)
- Número de documento (su formato también depende del tipo de documento seleccionado, TODO: se indica más adelante)
- Código entidad (es un código asignado por la entidad reguladora)
- Representante (funcionario dentro de la empresa)
- Flag Reporte (si se incluye como raíz del cálculo de acciones, TODO: se indica más adelante)
- País
- Ubicación interna en el país

#### Participación

- Sociedad (entidad Empresa)
- Accionista (entidad Empresa)
- Porcentaje de participación
- Fecha de vigencia

### Junta directiva

- Sociedad (entidad Empresa)
- Integrante (entidad Empresa, persona natural)
- Cargo principal (cargo que ocupa en la compañía)
- Cargo secundario (cargo que ocupa en la compañía)

## Análisis técnico

### Descripción

El proyecto estará dentro de un mismo

### Frontend

- Ya he creado el proyecto base en la carpeta `/app`
- Debe tener un sidebar en el lado izquierdo colapsable: cuando está colapsado se apreciarán solo los íconos de las opciones, y persistirá en `localStorage` vía Zustand
- Debe tener theme light/dark con persistencia en `localStorage` vía Zustand

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

#### Stack

- .NET 10
- C#
- MediatR
- FluentValidation (FluentValidation.DependencyInjectionExtensions)
- CQRS
- EF Core (versión compatible y estable más reciente)
- AutoMapper
- Microsoft Identity
- Directory.Packages

### Base de datos

- PostgreSQL (local: Docker; nube: puede ser Azure o Supabase)

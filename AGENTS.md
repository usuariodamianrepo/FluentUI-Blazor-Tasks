# FluentUI-Blazor-Tasks Architecture

## Overview

FluentUI-Blazor-Tasks is a task-assignment application built as a .NET 10 solution. It consists of a Fluent UI Blazor WebAssembly frontend, an ASP.NET Core Web API backend, and a shared project for contracts and types used across the application. SQL Server provides the application’s persistent data store.

## Solution Structure

The solution is organized into three projects under `src/`:

- **`src/FrontEnd.Blazor`** — The Blazor WebAssembly client application. It contains the user interface, client-side interaction, and calls to the backend API. Fluent UI Blazor components are used to build the application experience.
- **`src/BackEnd.API`** — The ASP.NET Core Web API. It exposes the server-side application endpoints and contains backend concerns such as request handling, application behavior, and persistence integration with SQL Server.
- **`src/Shared`** — Shared application types used by the frontend and backend, such as API contracts, request/response models, and common domain representations. Keeping these types in one project helps maintain consistency across the client-server boundary.

## Runtime Architecture

The Blazor WebAssembly client runs in the browser and communicates with `BackEnd.API` over HTTP. The API processes client requests, applies server-side application logic, and reads or writes data in SQL Server. Shared types from `Shared` define the data exchanged between the client and API.

The primary dependency direction is:

```text
FrontEnd.Blazor -> Shared
BackEnd.API     -> Shared
FrontEnd.Blazor -> BackEnd.API (HTTP)
BackEnd.API     -> SQL Server
```

The frontend should remain responsible for presentation and client interaction, while server-side validation, application behavior, and database access belong in the API. Shared types should represent the client-server contract without coupling the frontend directly to backend implementation details.

## Database Documentation

Database creation documentation and scripts are located in the `Documentation` folder. The database is intended to be initialized in SQL Server before using the application.

## AI tooling resources

- **Skills** — `.agents/skills/*/SKILL.md`: step-by-step task recipes. Scaffolders: `api-rest`, `ui-crud-splitter`
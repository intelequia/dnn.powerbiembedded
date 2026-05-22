# DNN Power BI Embedded — Copilot Instructions

## Project overview

This repo (`intelequia/dnn.powerbiembedded`) is a **DNN Platform** extension that embeds **Power BI / Microsoft Fabric** dashboards, reports, tiles and paginated reports into a DNN CMS site. It ships as a DNN installable module package (`.zip`) and exposes:

- An MVC module (`PowerBIEmbedded`) with three views: **ContentView**, **ListView**, **CalendarView**.
- A **DNN Persona Bar** admin extension (`Dnn.PowerBI`) built with React + Redux for managing workspaces, RLS, permissions, subscriptions and bookmarks.
- An **Extensibility** assembly (`DotNetNuke.PowerBI.Extensibility`) that lets third parties plug a custom RLS provider (`IRlsCustomExtension`).

Public documentation: https://github.com/intelequia/dnn.powerbiembedded/wiki and `docs/RLS-Configuration.md`, `docs/RLS-Examples.md`.

## Tech stack & constraints

- **.NET Framework 4.8** (full framework, NOT .NET Core / .NET 5+). Target stays `v4.8` — do not retarget.
- C# language features compatible with the project's csproj (no `record`s, no `file`-scoped namespaces unless already used).
- **DNN Platform 10.1.2** is the current build dependency (minimum runtime documented as 9.4.3). Persona Bar uses `Dnn.PersonaBar.Library` 10.1.2.
- **Microsoft.PowerBI.Api** 4.22.0 + `Microsoft.IdentityModel.Clients.ActiveDirectory` (ADAL) for Power BI REST + auth (master user *or* Service Principal).
- **Newtonsoft.Json 13**, ASP.NET MVC 5.3 / Web API 5.3, Web Pages / Razor 3.3.
- Front-end:
  - Persona Bar SPA: **React 16 + Redux + Webpack 4 + Babel 7** (`src/DotNetNuke.PowerBI/PBIEmbedded.Web`). Uses `@dnnsoftware/dnn-react-common` 9.11.
  - Module views: classic Razor `.cshtml` + jQuery + `powerbi-client` JS SDK (`src/DotNetNuke.PowerBI/scripts`, `Views/`).
- Database: SQL Server, versioned scripts under `Providers/DataProviders/SqlDataProvider/NN.NN.NN.SqlDataProvider`.

Manage NuGet via `packages.config` (not PackageReference). Both 9.4.3 and 10.1.2 DNN packages are present in `src/packages/` for compatibility checks — the csproj currently references the 10.1.2 set.

## Repository layout

```
src/
  DotNetNuke.PowerBI.sln                       # Open this in Visual Studio 2022
  DotNetNuke.PowerBI/                          # Main module project (assembly: DotNetNuke.PowerBI)
    DotNetNuke.PowerBI.dnn                     # DNN manifest — declares module, persona bar menu, SQL scripts
    Controllers/                               # MVC controllers (ContentView, ListView, CalendarView, Settings)
    Services/                                  # WebAPI controllers + EmbedService (Power BI token / embed config)
    Components/                                # FeatureController, RouteMapper, MenuController, Culture
    Models/                                    # EmbedConfig, TileEmbedConfig, Workspace, PowerBI* view models
    Data/                                      # Repositories (SharedSettings, Bookmarks, Subscriptions, ObjectPermissions)
    Providers/DataProviders/SqlDataProvider/   # Versioned install/uninstall SQL scripts
    Views/                                     # Razor views per module (ContentView, ListView, CalendarView, Settings, Shared)
    admin/personaBar/                          # Persona Bar host html + built JS/CSS bundle output
    PBIEmbedded.Web/                           # React/Redux source for the Persona Bar SPA
      src/{actions,reducers,store,containers,components,services,constants,globals,utils}
    Tasks/                                     # Scheduled tasks (e.g. SubscribeTask)
    App_LocalResources/                        # .resx for en-US / es-ES / de-DE
    BuildScripts/                              # Watch / packaging helpers
  DotNetNuke.PowerBI.Extensibility/            # Public extensibility contracts (IRlsCustomExtension)
  packages/                                    # NuGet packages restore folder
docs/                                          # RLS docs, screenshots, architecture diagram
Releases/                                      # Output: PowerBIEmbedded_<version>_Install.zip
```

## Build & run

Prereqs: Visual Studio 2022, Node + npm, a local DNN dev site (the React build expects one at `C:/websites/localhost.dnndev.me` — see `src/DotNetNuke.PowerBI/package.json` `dnn.dnnRoot`).

1. Install JS deps (Persona Bar SPA):
   ```powershell
   cd src\DotNetNuke.PowerBI\PBIEmbedded.Web
   npm install --force
   ```
2. **Debug**: build the C# solution in `Debug`, copy `bin\DotNetNuke.PowerBI.dll` (+ `.pdb`) to the DNN site `/bin`. Start the React dev server:
   ```powershell
   webpack-dev-server   # serves on https://localhost:8080 — Persona Bar loads bundle from there in Debug
   ```
3. **Release**: building the solution in `Release` runs the production webpack build and produces the installable package in `Releases\` (`PowerBIEmbedded_<version>_Install.zip`).
4. Known Node issue: if you see `error:0308010C:digital envelope routines::unsupported`, set `NODE_OPTIONS=--openssl-legacy-provider` before building.

Do **not** run `jupyter`, `dotnet` CLI builds, or convert this to SDK-style projects — the build pipeline depends on the classic csproj + MSBuild webpack invocation.

## Key architectural rules

### Power BI access
- Token acquisition + embed config generation lives in `Services/EmbedService.cs` (implements `IEmbedService`). Tokens are cached via `DotNetNuke.Services.Cache` and **expired slightly before the real expiry** to avoid `Forbidden` errors — preserve that behavior when editing.
- Two auth modes are supported per workspace settings: **Master User** (username/password via ADAL) and **Service Principal** (clientId/secret/tenantId). Paginated reports require Service Principal.
- DNN user **roles are always passed to Power BI** in the embed token (in addition to the RLS user identifier). See `docs/RLS-Configuration.md` — call this out in any RLS-related change.

### RLS extensibility
- Custom RLS providers implement `DotNetNuke.PowerBI.Extensibility.IRlsCustomExtension.GetRlsValue(HttpContext)`. Loaded by FQN from settings. Don't break the `IRlsCustomExtension` signature — it's a public contract consumed by external assemblies.

### Settings & persistence
- Multi-workspace per portal: `PowerBISettings` keyed by portal + settings id. Repositories in `Data/SharedSettings/`, `Data/Bookmarks/`, `Data/Subscriptions/` follow a `*Repository` + `I*Repository` singleton pattern (`Instance` property).
- Any schema change requires a **new** numbered SQL script in `Providers/DataProviders/SqlDataProvider/` AND a matching `<script type="Install">` entry in `DotNetNuke.PowerBI.dnn`. Never modify a published script; add a new versioned one.
- Object-level permissions go through `ObjectPermissionsRepository` / `IObjectPermissionsRepository`. Inheritance from workspace permissions exists (see release notes 1.0.6, 1.0.7, 1.2.1) — preserve it.

### Web API routing
- `Components/RouteMapper.cs` registers two route namespaces:
  - `DotNetNuke.PowerBI.Controllers.Api.Admin` (module-facing API)
  - `DotNetNuke.PowerBI.Services` (persona bar API — controllers decorated with `[MenuPermission(Scope = ServiceScope.Admin)]`)
- New persona bar endpoints belong in `Services/` and must keep the `MenuPermission` attribute.

### Localization
- All user-facing strings go through `.resx` files under `App_LocalResources/` and `admin/personaBar/App_LocalResources/`. Supported cultures: `en-US`, `es-ES`, `de-DE`. Add keys to all three when adding new strings.

### Front-end (Persona Bar SPA)
- React class components + Redux (actions / reducers / store). Use `@dnnsoftware/dnn-react-common` widgets to match DNN look & feel. The webpack entry is `src/main.jsx`; production build outputs into `admin/personaBar/scripts/bundles/` and is packaged via the manifest's `PersonaBarResources.zip`.
- ESLint is enforced (`.eslintrc.js`). Run `npm run lint` before submitting changes.

### Module front-end (Razor views)
- The Power BI JS client (`powerbi-client`) is loaded from `scripts/`. Multiple ContentView modules can live on the same page — keep component state scoped (don't introduce global singletons in JS).

## Coding conventions

- Namespace root: `DotNetNuke.PowerBI.*`. Mirror folder structure.
- Use `DotNetNuke.Instrumentation.ILog` (`LoggerSource.Instance.GetLogger(typeof(...))`) for logging — not `System.Diagnostics` or `ILogger<T>`.
- WebAPI controllers return `HttpResponseMessage` via `Request.CreateResponse(...)` / `CreateErrorResponse(...)`; follow the existing `try/catch -> Logger.Error -> 500` pattern.
- Prefer 4-space indentation, Allman braces, `var` for locals when the type is obvious (matches existing style).
- Don't introduce async-only APIs into synchronous DNN call sites; the Power BI SDK is async — wrap with care and avoid `.Result` deadlocks (use `ConfigureAwait(false)` where you must block).
- Keep copyright header style (the MIT block at the top of `PBIEmbeddedController.cs`) on new C# files where the rest of the folder uses it.

## Release / versioning

- Bump the `version` attribute in `DotNetNuke.PowerBI.dnn` (`<package ... version="MM.mm.bb">`) and add a `<b>Version X.Y.Z</b>` block in `ReleaseNotes.txt` for every release. The packaged zip filename is driven by this version.
- New SQL scripts must be added to the manifest as described above.

## When making changes

- If touching anything Power BI / Azure related, prefer the official Microsoft.PowerBI.Api SDK over hand-rolled REST calls. Use `microsoft_docs_search` / `microsoft_docs_fetch` for current Power BI REST + embedding guidance, and `github_repo` against `intelequia/dnn.powerbiembedded` for cross-referencing existing patterns.
- If touching Azure resources or guidance, follow Azure best practices.
- Don't add features, comments, docstrings, or refactors that weren't requested.
- Don't introduce new top-level dependencies (NuGet or npm) without flagging it — this module ships into customer DNN sites and dependency surface matters.

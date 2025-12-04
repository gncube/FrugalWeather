# Copilot Instructions for FrugalWeather

Purpose: Help an AI coding agent become productive quickly in this repository. Focus is the Azure Functions API in `src/API` and the CI workflow.

Quick orientation

- This repository hosts a small Blazor client and a .NET 10 Azure Functions API. The Functions API lives in `src/API` and uses the .NET isolated worker model (FUNCTIONS_WORKER_RUNTIME=dotnet-isolated). Key files:
  - `src/API/Program.cs` — application startup using `FunctionsApplication.CreateBuilder`
  - `src/API/*.cs` — function handlers: `GetWeather.cs`, `GetForecast.cs`
  - `src/API/host.json` and `src/API/local.settings.json` — local runtime configuration
  - `src/API/FrugalWeather.API.csproj` — project SDK/Dependencies (net10.0, Azure Functions v4)
  - `.github/workflows/main_func-frugalweather.yml` — CI: builds `src/API` and deploys to Azure

Big-picture architecture

- The API exposes two HTTP-triggered functions that return mock weather data. There is no database or external API integration in this service; state is ephemeral and generated in-memory.
- Telemetry: Application Insights is configured in `Program.cs` via `AddApplicationInsightsTelemetryWorkerService`.
- Deployment: CI publishes `dotnet publish` output from `src/API/output` and uses the Azure/functions-action to push to `func-frugalweather`.

Developer workflows (explicit commands)

- Local run (Functions Tools):
  - From repository root: `func start --script-root src/API` (ensure `func` CLI installed)
- Local debugging: Open `src/API` in VS Code and use the Functions extension; `local.settings.json` controls local connections
- Build/publish locally:
  - `dotnet publish src/API -c Release -o src/API/output`
  - `func azure functionapp publish func-frugalweather` (from `src/API`)
- CI: The GitHub Actions workflow at `.github/workflows/main_func-frugalweather.yml` runs on `main` and uses `actions/setup-dotnet` then `dotnet publish` and `Azure/functions-action` with a publish profile secret.

Important conventions & patterns

- Worker model: This project uses .NET isolated worker (`dotnet-isolated`) with Azure Functions v4; any added functions should follow this pattern (Function attribute on public methods in classes registered by DI).
- Logging: Use `ILogger<T>` injection; functions return `IActionResult` and use `BadRequestObjectResult` / `OkObjectResult` patterns.
- Config: Keep `local.settings.json` for local-only values; do NOT commit production secrets. For deployed apps, set `AzureWebJobsStorage` in Function App settings (CI does not manage this secret). `UseDevelopmentStorage=true` is only for local emulation and causes sync errors when deployed.

Integration points & observability

- Azure resources: `func-frugalweather` Function App and `stfrugalweather` Storage Account (see `docs/SETUP.md` for CLI create commands).
- Application Insights: configured in `Program.cs`; ensure the instrumentation key / connection string is available in App Settings when deployed.

What to watch for when editing

- Avoid changing `FUNCTIONS_WORKER_RUNTIME` or removing `Microsoft.Azure.Functions.Worker` packages; those are required for isolated worker apps.
- Keep endpoints stable: routes are defined on the `HttpTrigger` attribute (`Route = "weather"` and `weather/forecast`). Changing routes requires updating docs and client code.
- `local.settings.json` should remain for local development and must not be used as production configuration.

Files to reference when implementing features or fixes

- `src/API/Program.cs` — startup and DI
- `src/API/FrugalWeather.API.csproj` — add package references here
- `src/API/GetWeather.cs`, `src/API/GetForecast.cs` — examples of HTTP-triggered functions and response models
- `.github/workflows/main_func-frugalweather.yml` — CI publish pipeline
- `docs/SETUP.md` — environment provisioning steps (resource group, storage account, functionapp); update if you change deployment assumptions

Example tasks for an AI agent

- Add a new HTTP-triggered function: follow `GetWeather.cs` structure, register any services in `Program.cs`, add tests (if added later), and update docs.
- Fix trigger sync failures: check `local.settings.json` and ensure `AzureWebJobsStorage` is set in production app settings. Use `az functionapp config appsettings set` to apply.
- Update CI to include integration tests: modify `.github/workflows/main_func-frugalweather.yml` to run tests before publish and fail fast.

If uncertain, ask the maintainer

- Which secrets should live in GitHub Actions secrets (publish profile already used)?
- Are you ok with switching to feature-branch PR workflow for CI changes?

Short checklist before opening a PR

- Build locally with `dotnet publish src/API -c Release -o src/API/output`
- Run `func start --script-root src/API` and exercise endpoints
- Update `docs/SETUP.md` if provisioning or deployment steps change

If this file already existed, merge note:

- Preserve any high-level policies and contact points. Replace generic advice with the concise, repository-specific checklist above.

---

Please review this draft and tell me any missing integration points, required secrets, or developer workflows to add. I'll iterate until it's complete.

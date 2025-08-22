# http-files-test

Minimal ASP.NET Core (.NET 9) API with CRUD endpoints for forecasts using in-memory storage. Includes an HTTP file (forecast.http) for quick testing in Visual Studio.

## Prerequisites
- .NET 9 SDK
- Visual Studio 2022 (or `dotnet` CLI)

## Run
- Visual Studio: Set http-files-test as startup project and press F5.
- CLI:

- The app prints the application URLs on start (e.g., http://localhost:5000). OpenAPI is available in Development at `/openapi/v1.json`.


## Endpoints
- GET `/forecasts` → 200 OK, list of forecasts.
- GET `/forecasts/{id}` → 200 OK or 404 Not Found.
- POST `/forecasts` → 201 Created with created resource.
- Body (application/json): `{ "date": "YYYY-MM-DD", "temperatureC": 12, "summary": "Mild" }`
- PUT `/forecasts/{id}` → 200 OK or 404 Not Found.
- Body: same shape as POST (replace entire resource).
- PATCH `/forecasts/{id}` → 200 OK or 404 Not Found.
- Body: any subset, e.g. `{ "temperatureC": 22 }`
- DELETE `/forecasts/{id}` → 204 No Content or 404 Not Found.
- GET `/weatherforecast` → Sample data (unchanged demo endpoint).

Note: Data is stored in-memory (`ConcurrentDictionary`) and resets on each run.

## HTTP file (recommended)
Use `forecast.http` for quick requests in Visual Studio:
1. Open `forecast.http`.
2. Set `@baseUrl` to your app URL from the debug output.
3. Send the POST request first, copy the returned id, and set `@id` for subsequent requests.

## cURL examples
````````

## Project structure
- `Program.cs` — minimal API, endpoints, and models.
- `forecast.http` — ready-to-run HTTP requests for testing.

# Response
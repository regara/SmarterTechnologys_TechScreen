# Smarter Technology Package Sorter

Production-quality C# solution for sorting factory packages by volume, dimensions, and mass.

## Objective

Implement `sort(width, height, length, mass) -> string` so each package is dispatched to:

- `STANDARD`
- `SPECIAL`
- `REJECTED`

according to the exact rules below.

## Rules Implemented

- `bulky` when `width * height * length >= 1_000_000` cm^3, or any dimension is `>= 150` cm
- `heavy` when `mass >= 20` kg

Dispatch logic:

- `STANDARD`: not bulky and not heavy
- `SPECIAL`: bulky or heavy (but not both)
- `REJECTED`: bulky and heavy

## Function Contract

The required function exists in the core library:

- `PackageSorter.sort(double width, double height, double length, double mass) : string`
- Units: dimensions in centimeters, mass in kilograms

It returns one of:

- `STANDARD`
- `SPECIAL`
- `REJECTED`

Source:

- `SmarterTechnology.PackageSorter/PackageSorter.cs`

## Repository Structure

- `SmarterTechnology.PackageSorter`
  - Core sorting logic and stack type model
- `SmarterTechnology.PackageSorter.Web`
  - ASP.NET Core web app for entering packages and visualizing stack assignment
- `SmarterTechnology.PackageSorter.Tests`
  - MSTest + FluentAssertions test suite for correctness and edge-case handling

## Edge Cases and Input Validation

Core validation intentionally enforces safe numeric inputs:

- `width`, `height`, `length` must be finite and `> 0`
- `mass` must be finite and `>= 0`
- `NaN` / `Infinity` values are rejected with clear exceptions
- Decimal values are supported

In the web app, invalid inputs are surfaced as validation errors from the API.

## How To Run

### Web UI

```bash
dotnet run --project .\SmarterTechnology.PackageSorter.Web
```

Then open the URL printed by ASP.NET (typically `http://localhost:5000` and/or `https://localhost:5001`).

UI features:

- Enter package dimensions and mass
- Add multiple packages in one session
- Live totals
- Grouped lists by `STANDARD`, `SPECIAL`, `REJECTED`

### Optional API Check

```bash
curl -X POST http://localhost:5000/api/sort ^
  -H "Content-Type: application/json" ^
  -d "{\"width\":100,\"height\":100,\"length\":100,\"mass\":20}"
```

Expected response:

```json
{"stack":"REJECTED"}
```

## Tests

Run:

```bash
dotnet test .\SmarterTechnology.PackageSorter.Tests\SmarterTechnology.PackageSorter.Tests.csproj
```

Test coverage includes:

- Required stack routing behavior
- Threshold boundaries (`>=` behavior at 150 cm, 1,000,000 cm^3, 20 kg)
- Decimal values
- Invalid and non-finite input handling

## Evaluation Criteria Checklist

- Correct sorting logic: implemented and tested
- Code quality: separated core logic, web host, and tests
- Edge cases and inputs: validated explicitly
- Test coverage: includes nominal, boundary, and failure scenarios

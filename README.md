# Smarter Technology Package Sorter

Interview-ready C# solution for sorting factory packages by dimension, volume, and mass.

## Problem Summary

Implement:

`sort(width, height, length, mass) -> string`

Rules:

- `bulky` if `width * height * length >= 1,000,000 cm^3` OR any dimension `>= 150 cm`
- `heavy` if `mass >= 20 kg`

Return stack:

- `STANDARD` when neither bulky nor heavy
- `SPECIAL` when either bulky or heavy
- `REJECTED` when both bulky and heavy

## Project Structure

- `SmarterTechnology.PackageSorter` - core sorting library
- `SmarterTechnology.PackageSorter.Web` - ASP.NET Core web UI for adding packages and viewing live stack groups
- `SmarterTechnology.PackageSorter.Tests` - MSTest + FluentAssertions tests

## Edge Case Handling

The core sorter rejects invalid input values:

- Dimensions must be finite and greater than `0`
- Mass must be finite and greater than or equal to `0`
- Non-finite values (`NaN`, `Infinity`) throw clear exceptions

Decimal values are fully supported.

## Run Web UI

```bash
dotnet run --project .\SmarterTechnology.PackageSorter.Web
```

Then open the local URL shown in the console (typically `http://localhost:5000` or `https://localhost:5001`).

Web UI capabilities:

- Add packages from explicit width/height/length/mass fields
- Keep adding multiple packages
- See running totals and grouped stack lists (`STANDARD`, `SPECIAL`, `REJECTED`)

## Test

```bash
dotnet test .\SmarterTechnology.PackageSorter.Tests\SmarterTechnology.PackageSorter.Tests.csproj
```

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
- `SmarterTechnology.PackageSorter.Demo` - console app with `.Main` entry point and user-friendly input flow
- `SmarterTechnology.PackageSorter.Tests` - xUnit + FluentAssertions tests

## Edge Case Handling

The core sorter rejects invalid input values:

- Dimensions must be finite and greater than `0`
- Mass must be finite and greater than or equal to `0`
- Non-finite values (`NaN`, `Infinity`) throw clear exceptions

Decimal values are fully supported.

## Run

Interactive mode:

```bash
dotnet run --project .\SmarterTechnology.PackageSorter.Demo
```

Command-line mode:

```bash
dotnet run --project .\SmarterTechnology.PackageSorter.Demo -- 100 100 100 20
```

## Test

```bash
dotnet test .\SmarterTechnology.PackageSorter.Tests\SmarterTechnology.PackageSorter.Tests.csproj
```

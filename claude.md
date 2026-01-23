# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

Verify is a snapshot testing framework for .NET that simplifies assertion of complex data models and documents. It serializes test results to `.verified.*` files and compares them against actual results on subsequent runs.

**Solution file**: `src\Verify.slnx` (uses modern .slnx format)

## Build & Test Commands

### Building the Solution

The main solution file is `src\Verify.slnx` (note the .slnx extension, not .sln).

```bash
# Build entire solution
dotnet build src/Verify.slnx --configuration Release

# Build dangling snapshots solution (separate)
dotnet build src/VerifyDangling.slnx --configuration Release
```

### Running Tests

```bash
# Run all tests for a specific test project
dotnet test src/Verify.Xunit.Tests --configuration Release
dotnet test src/Verify.NUnit.Tests --configuration Release
dotnet test src/Verify.MSTest.Tests --configuration Release

# Run a single test (example)
dotnet test src/Verify.Tests --filter "FullyQualifiedName~YourTestName"

# Run tests that require direct execution (TUnit)
dotnet run --project src/Verify.TUnit.Tests/Verify.TUnit.Tests.csproj --configuration Release
```

### Common Test Projects

- `Verify.Tests` - Core verification engine tests
- `Verify.Xunit.Tests` / `Verify.XunitV3.Tests` - xUnit adapter tests
- `Verify.NUnit.Tests` - NUnit adapter tests
- `Verify.MSTest.Tests` - MSTest adapter tests
- `Verify.Fixie.Tests` - Fixie adapter tests
- `Verify.Expecto.Tests` / `Verify.Expecto.FSharpTests` - Expecto (F#) adapter tests
- `Verify.TUnit.Tests` - TUnit adapter tests
- `DeterministicTests` - Deterministic serialization tests
- `StaticSettingsTests` - Global configuration tests
- `StrictJsonTests` - JSON parsing tests

### Tool Installation

```bash
# Restore dotnet tools (required for Fixie tests)
dotnet tool restore --tool-manifest src/.config/dotnet-tools.json
```

## Architecture

### Three-Layer Design

1. **Test Framework Adapters** (`Verify.NUnit`, `Verify.Xunit`, `Verify.MSTest`, etc.)
   - Extract test metadata (type name, method name, parameters) from framework context
   - Implement `BuildVerifier()` to create `InnerVerifier` instances
   - Each adapter is framework-specific but follows the same pattern

2. **Core Verification Engine** (`Verify` project)
   - `InnerVerifier` - Orchestrates verification workflow (split across 12 partial files by data type)
   - `VerifyEngine` - Comparison logic, diff tool launching, callback execution
   - `SettingsTask` - Fluent builder API for configuration

3. **Serialization & Comparison**
   - `VerifyJsonWriter` - Custom JSON writer extending Argon library
   - `SerializationSettings` - Registry of converters and scrubbers
   - `Comparer` / `FileComparer` / `StreamComparer` - Different comparison strategies

### Key Design Patterns

**Adapter Pattern**: Each test framework has a minimal adapter that extracts framework metadata differently:
- **Xunit**: Uses `UseVerifyAttribute` (a `BeforeAfterTestAttribute`) injected via MSBuild
- **NUnit**: Direct access to `TestContext.CurrentContext.Test`
- **MSTest**: Requires `[UsesVerify]` attribute with source generator
- **Fixie**: Uses `ExecutionState.Current`

**Partial Classes**: `InnerVerifier` is split across 12 files:
- `InnerVerifier_Object.cs` - Object serialization
- `InnerVerifier_String.cs` - Text comparison
- `InnerVerifier_Stream.cs` - Binary files
- `InnerVerifier_Json.cs` - JSON handling
- `InnerVerifier_Xml.cs` - XML serialization
- `InnerVerifier_Archive.cs` - ZIP files
- `InnerVerifier_Directory.cs` - Directory trees
- `InnerVerifier_File.cs` - Individual files
- `InnerVerifier_Task.cs` - Async unwrapping
- `InnerVerifier_Throws.cs` - Exception capture
- And others...

**Builder Pattern**: `SettingsTask` provides fluent API with ~50 configuration methods that are lazily evaluated at verification time.

**Counter Pattern**: Deduplicates repeated values in filenames:
- First occurrence: `Date`, second: `Date_1`, third: `Date_2`, etc.
- Separate counters for DateTime, DateTimeOffset, Guid, etc.

### File Naming Convention

Format: `{TypeName}.{MethodName}_{Parameters}_{UniqueFor}.{Status}.{Extension}`

Example: `MyTests.MyTest_param1_param2.verified.txt`

Key components:
- `Namer.cs` - Central naming logic with "unique for" variants (runtime, framework, etc.)
- `FileNameBuilder.cs` - Builds filename segments from test metadata
- `ParameterToName` - Converts parameters to filename-safe strings
- `FileNameCleaner.cs` - Removes invalid characters (`: * ? / < > |`)

### Critical Code Locations

- **Verification flow**: `src/Verify/Verifier/VerifyEngine.cs` (main orchestrator)
- **Serialization**: `src/Verify/Serialization/VerifyJsonWriter.cs` and `SerializationSettings.cs`
- **Comparison logic**: `src/Verify/Compare/Comparer.cs`
- **File naming**: `src/Verify/Naming/FileNameBuilder.cs`
- **Test adapter examples**: `src/Verify.Xunit/Verifier.cs`, `src/Verify.NUnit/Verifier.cs`

## Important Conventions

### Snapshot Files

- **Exclude from source control**: All `*.received.*` files (mismatches during test runs)
- **Include in source control**: All `*.verified.*` files (approved snapshots)
- **File characteristics**: UTF-8 with BOM, LF line endings, no trailing newline

### .gitattributes Requirements

```
*.received.*
*.verified.txt text eol=lf working-tree-encoding=UTF-8
*.verified.xml text eol=lf working-tree-encoding=UTF-8
*.verified.json text eol=lf working-tree-encoding=UTF-8
*.verified.bin binary
```

### EditorConfig Settings

Verify files should use:
- `charset = utf-8-bom`
- `end_of_line = lf`
- `insert_final_newline = false`

## Project Structure

```
src/
├── Verify/                          # Core library
│   ├── Verifier/                    # Verification engine
│   ├── Naming/                      # File naming logic
│   ├── Serialization/               # JSON serialization & converters
│   ├── Compare/                     # Comparison strategies
│   ├── Recording/                   # Data recording during tests
│   ├── Combinations/                # Cartesian product testing
│   ├── DerivePaths/                 # Build metadata integration
│   └── buildTransitive/             # MSBuild props/targets
├── Verify.NUnit/                    # NUnit adapter
├── Verify.Xunit/                    # xUnit v2 adapter
├── Verify.XunitV3/                  # xUnit v3 adapter
├── Verify.MSTest/                   # MSTest adapter
├── Verify.MSTest.SourceGenerator/   # Code generation for MSTest
├── Verify.Fixie/                    # Fixie adapter
├── Verify.Expecto/                  # Expecto (F#) adapter
├── Verify.TUnit/                    # TUnit adapter
├── [Various test projects]
└── .config/dotnet-tools.json        # Local tool manifest
```

## Development Workflow

### Adding a Test Framework Adapter

1. Create new project (e.g., `Verify.NewFramework`)
2. Implement `BuildVerifier()` to extract test context from framework
3. Create partial `Verifier` classes for each verification method (Object, String, Stream, etc.)
4. Add MSBuild props file to inject any required attributes/configuration
5. Optionally add module initializer to register test attachment callbacks

Reference existing adapters like `Verify.Xunit/Verifier.cs` or `Verify.NUnit/Verifier.cs`.

### Extending Serialization

1. Create converter class implementing appropriate interface in `src/Verify/Serialization/Converters/`
2. Register via `VerifierSettings.AddSerializer<T>(converter)` in module initializer
3. Converters can access `SerializationSettings` for configuration

### Modifying File Naming

- Primary logic in `src/Verify/Naming/FileNameBuilder.cs`
- Parameter conversion in `src/Verify/Naming/ParameterToName.cs`
- Sanitization in `src/Verify/FileNameCleaner.cs`

### Debugging Tests

When tests fail, `.received.*` files are created showing actual output. Compare with `.verified.*` files to understand differences. On CI (AppVeyor), failed test artifacts are uploaded for inspection.

## Multi-Targeting

The solution supports:
- **.NET Framework**: net462, net472, net48, net481 (Windows only)
- **.NET**: net6.0, net7.0, net8.0, net9.0, net10.0

SDK version: 10.0.102 (see `src/global.json`)

Platform-specific code uses conditional compilation:
```csharp
#if NET6_0_OR_GREATER
// Modern .NET code
#endif

#if NET8_0_OR_GREATER
// .NET 8+ optimizations (e.g., SearchValues<T>)
#endif
```

## Common Gotchas

1. **Module Initializers**: Global configuration via `VerifierSettings` must be done in a module initializer (`[ModuleInitializer]`) before any tests run.

2. **MSTest Requires Opt-in**: MSTest tests need `[UsesVerify]` attribute on class/assembly, or inherit from `VerifyBase`.

3. **Fixie Requires Custom Convention**: Fixie needs `ITestProject` and `IExecution` implementations that call `VerifierSettings.AssignTargetAssembly()` and `ExecutionState.Set()`.

4. **Snapshot File Nesting**: Verify includes MSBuild props that nest snapshot files under test files in IDE. Disable with `<DisableVerifyFileNesting>true</DisableVerifyFileNesting>`.

5. **TUnit Tests**: Use `dotnet run` instead of `dotnet test` for TUnit projects.

6. **Tool Restoration**: Run `dotnet tool restore` before running Fixie tests (requires fixie.console tool).

## Source Generator (MSTest)

The MSTest adapter includes a source generator at `src/Verify.MSTest.SourceGenerator/` that generates code for tests marked with `[UsesVerify]`. This handles test context plumbing automatically.

## Documentation Generation

README and docs are generated from source files using [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets):
- Source: `readme.source.md`
- Generated: `readme.md` (DO NOT EDIT)
- Includes from: `docs/mdsource/*.include.md`

Code snippets are embedded from actual test files using special markers.

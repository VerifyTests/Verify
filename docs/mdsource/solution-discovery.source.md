# Solution Discovery

Verify automatically discovers solution information at build time and embeds it into the test assembly as metadata. This metadata is used to determine where to store snapshot files.

This is used for [Directory Scrubbing](/scrubbers.md#directory-scrubbers)


## How It Works

During the build process, Verify searches for solution files (`.slnx` or `.sln`) in the following locations (relative to the project directory):

1. Project directory
2. Parent directory
3. Parent's parent directory

If a single solution file is found, Verify extracts and stores:

- `SolutionDir` - The directory containing the solution file
- `SolutionName` - The solution file name without extension

This information is embedded in the assembly as `AssemblyMetadataAttribute` values with keys:

- `Verify.SolutionDirectory`
- `Verify.SolutionName`


## Preference Rules

- If both `.slnx` and `.sln` files exist in the same directory, `.slnx` is preferred
- If multiple `.slnx` files exist, Verify cannot auto-discover and will show a warning
- If no `.slnx` files exist but multiple `.sln` files exist, Verify cannot auto-discover and will show a warning


## Explicit Override

Explicitly set solution information via MSBuild command-line properties:

```bash
dotnet build /p:SolutionDir="C:\Path\To\Solution\" /p:SolutionName="MySolution"
```

When `SolutionDir` is explicitly provided but `SolutionName` is not, Verify will attempt to derive `SolutionName` from the solution file found in the specified `SolutionDir`.


## Warning Messages

If Verify cannot discover solution information, it will emit build warnings with guidance:


### Multiple Solution Files Found

```
Multiple solution files found. Unable to auto-discover SolutionDir and SolutionName.
Found: C:\Path\Solution1.slnx;C:\Path\Solution2.slnx;C:\Path\Solution.sln.
Verify searches for .slnx and .sln files in the project directory, parent directory, and parent's parent directory.
To resolve this, either ensure only one solution file exists in these locations, or explicitly set SolutionDir and SolutionName via command line:
/p:SolutionDir="C:\Path\To\Solution\" /p:SolutionName="MySolution"
```


### No Solution Files Found

```
No solution files found. Unable to auto-discover SolutionDir and SolutionName.
Verify searches for .slnx and .sln files in the project directory '(C:\Path\To\Project)', parent directory, and parent's parent directory.
To resolve this, either add a solution file to one of these locations, or explicitly set SolutionDir and SolutionName via command line:
/p:SolutionDir="C:\Path\To\Solution\" /p:SolutionName="MySolution"
```


### Multiple Solution Files in Explicit SolutionDir

```
Multiple solution files found in SolutionDir 'C:\Path\To\Solution\'. Unable to auto-discover SolutionName.
Found: C:\Path\To\Solution\Solution1.slnx;C:\Path\To\Solution\Solution2.sln.
To resolve this, explicitly set SolutionName via command line:
/p:SolutionName="MySolution"
```


## Build Integration

Solution discovery happens automatically as part of the build via the `Verify.props` MSBuild targets file, which is automatically imported when referencing the Verify NuGet package.

The discovery runs in the `DiscoverSolutionInfo` target, which executes before compilation, and the discovered values are written to generated source files by the `WriteVerifyAttributes` target.

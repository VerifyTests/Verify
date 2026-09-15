[Microsoft.Testing.Platform](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-intro) (MTP) is the recommended way to run tests. It replaces VSTest, so `Microsoft.NET.Test.Sdk` is not required.

### dotnet test

The .NET 10 SDK added an [MTP mode to dotnet test](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test). The default VSTest mode does not run MTP test projects. Enable MTP mode by adding the following to `global.json` at the root of the repository:

```json
{
  "test": {
    "runner": "Microsoft.Testing.Platform"
  }
}
```

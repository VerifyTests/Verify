The MSTest implementation leverages a [Source Generator](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) and requires test classes to opt in to being processed by the Source Generator.

Add the `UsesVerifyAttribute`.

For all test classes in an assembly:

```
[assembly: UsesVerify]
```

For a specific a test class:

```
[UsesVerify]
[TestClass]
public class TheTest...
```

Or inherit from `VerifyBase`:

snippet: VerifyBaseUsage.cs
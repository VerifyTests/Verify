The MSTest implementation leverages a [Source Generator](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) and requires test classes to opt in to being processed by the Source Generator.

Add the `UsesVerifyAttribute` to mark a test class or all test classes in an assembly:

```
[assembly: UsesVerify]
```

```
[UsesVerify]
```

Or inherit from `VerifyBase`:

snippet: VerifyBaseUsage.cs
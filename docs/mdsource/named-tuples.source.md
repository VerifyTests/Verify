# Named Tuples

Instances of [named tuples](https://docs.microsoft.com/en-us/dotnet/csharp/tuples#named-and-unnamed-tuples) can be verified.

Due to the use of [ITuple](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.ituple), this approach is only available an net472+ and netcoreapp2.2+.

Given a method that returns a named tuple:

snippet: MethodWithNamedTuple

Can be verified:

snippet: VerifyTuple

Resulting in:

snippet: SerializationTests.NamedTuple.verified.txt
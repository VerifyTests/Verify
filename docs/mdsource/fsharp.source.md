# Usage in FSharp

Argon ships F# support in a separate nuget [Argon.FSharp](https://www.nuget.org/packages/Argon.FSharp/).

To serialize F# types properly, add the converters:

```fs
VerifierSettings.AddExtraSettings(fun settings -> settings.AddFSharpConverters())
```


## NullValueHandling

By default [DefaultValueHandling is Ignore](/docs/serializer-settings.md#default-settings). Since F# `Option.None` is treated as null, it will be ignored by default. To include `Option.None` use `VerifierSettings.AddExtraSettings` at module startup:

snippet: NullValueHandling


## Async Qwerks

F# does not respect implicit operator conversion. `SettingsTask` uses implicit operator conversion to provide a fluent api in combination with an instance that can be awaited. As such `SettingsTask.ToTask()` needs to be awaited when used inside F#.

snippet: FsTest


## Full tests

snippet: FSharpTests/Tests.fs
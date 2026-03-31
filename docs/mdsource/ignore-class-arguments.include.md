### Ignore class arguments

`VerifierSettings.IgnoreClassArguments()` can be used to globally ignore class constructor arguments from the verified filename. This is useful when infrastructure fixtures (e.g. TUnit's `ClassConstructor` or NUnit's `TestFixtureSource`) are injected via the constructor and should not affect snapshot file names. It must be called before any test runs, typically in a `[ModuleInitializer]`.

The received files still contain all class argument values.

```cs
[ModuleInitializer]
public static void Init() =>
    VerifierSettings.IgnoreClassArguments();
```

`IgnoreClassArguments` can also be used at the test level:

```cs
await Verify(result).IgnoreClassArguments();
```

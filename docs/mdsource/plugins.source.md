# Plugins

Verify can be extended using plugins that are shipped as nuget packages.


## Extension points

Plugins usually manipulate a subset of the extension points of Verify.

 * [Comparers](comparer.md) for comparing non-text files.
 * [Converters](converter.md) to split a target into its component parts, then verify each of those parts.
 * [Serializer settings](serializer-settings.md)
 * [Ordering](ordering.md)
 * [Scrubbers](scrubbers.md)


## Enabling Plugins

Plugins can have static config and/or instance based APIs


### Static config

Static APIs are enabled in a ModuleInitializer of the consuming test project.

For example in [Verify.Http](https://github.com/VerifyTests/Verify.Http) the method `VerifyHttp.Initialize()` is called:

```
public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize() =>
        VerifyHttp.Initialize();
}
```

### Automatic plugin detection

There is a convenience API for initializing all plugins: `VerifierSettings.InitializePlugins()`

This API performs the following

 * Scans the current assembly's directory for plugin assemblies matching `Verify.*.dll`
 * Loads each assembly
 * Try to find the type `VerifyTests.[AssemblyNameWithPeriodsRemove]`
 * If no type exists move to the next assembly.
 * If the `Initialized` property ('public static bool Initialized { get; }') is true then move to the next assembly. An exception is thrown if no `Initialized` property is found.
 * Invoke the `Initialize` method (`public static void Initialize()`) on that type. An exception is thrown if no `Initialize` method is found. Optional parameters are supported.

The plugins `Initialize` method should throw if already Initialized and set `Initialized` to `true`:

```
public static void Initialize()
{
    if (Initialized)
    {
        throw new("Already Initialized");
    }

    Initialized = true;
```

`VerifierSettings.InitializePlugins` is called in a ModuleInitializer of the consuming test project:

```
public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize() =>
        VerifierSettings.InitializePlugins();
}
```


#### Combining specific config with automatic config

In some scenarios it may be necessary to explicitly call Initialize for some plugins. For example when order is important, or a certain plugin has some parameters on Initialize. In this case the specific plugin(s) Initialize methods can be called, then InitializePlugins can be call.

```
public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyNServiceBus.Initialize(captureLogs: true);
        VerifierSettings.InitializePlugins();
    }
}
```


## Existing Plugins

include: plugin-list
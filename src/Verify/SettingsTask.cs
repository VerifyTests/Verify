using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace VerifyTests;

public class SettingsTask
{
    VerifySettings? settings;
    Func<VerifySettings, Task> buildTask;
    Task? task;

    public SettingsTask(VerifySettings? settings, Func<VerifySettings, Task> buildTask)
    {
        if (settings is not null)
        {
            this.settings = new(settings);
        }

        this.buildTask = buildTask;
    }

    public SettingsTask AddExtraSettings(Action<JsonSerializerSettings> action)
    {
        CurrentSettings.AddExtraSettings(action);
        return this;
    }

    /// <summary>
    /// Retrieves the value passed into <see cref="UseExtension"/>, if it exists.
    /// </summary>
    public bool TryGetExtension([NotNullWhen(true)] out string? extension)
    {
        return CurrentSettings.TryGetExtension(out extension);
    }

    /// <summary>
    /// Define the parameter values being used by a parameterised (aka data drive) test.
    /// In most cases the parameter parameter values can be automatically resolved.
    /// When this is not possible, an exception will be thrown instructing the use of <see cref="UseParameters"/>
    /// Not compatible with <see cref="UseTextForParameters"/>.
    /// </summary>
    public SettingsTask UseParameters(params object?[] parameters)
    {
        CurrentSettings.UseParameters(parameters);
        return this;
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public SettingsTask AddScrubber(Action<StringBuilder> scrubber)
    {
        CurrentSettings.AddScrubber(scrubber);
        return this;
    }

    /// <summary>
    /// Modify the resulting test content using custom code.
    /// </summary>
    public SettingsTask AddScrubber(string extension, Action<StringBuilder> scrubber)
    {
        CurrentSettings.AddScrubber(extension, scrubber);
        return this;
    }

    /// <summary>
    /// Replace inline <see cref="Guid"/>s with a placeholder.
    /// Uses a <see cref="Regex"/> to find <see cref="Guid"/>s inside strings.
    /// </summary>
    public SettingsTask ScrubInlineGuids()
    {
        CurrentSettings.ScrubInlineGuids();
        return this;
    }

    public SettingsTask UseStreamComparer(StreamCompare compare)
    {
        CurrentSettings.UseStreamComparer(compare);
        return this;
    }

    public SettingsTask UseStringComparer(StringCompare compare)
    {
        CurrentSettings.UseStringComparer(compare);
        return this;
    }

    /// <summary>
    /// Disable using a diff toll for this test
    /// </summary>
    public SettingsTask DisableDiff()
    {
        CurrentSettings.DisableDiff();
        return this;
    }

    /// <summary>
    /// Use the current runtime to make the test results unique.
    /// Used when a test produces different results based on runtime.
    /// </summary>
    public SettingsTask UniqueForRuntime()
    {
        CurrentSettings.UniqueForRuntime();
        return this;
    }

    /// <summary>
    /// Use the current test assembly TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    public SettingsTask UniqueForTargetFramework()
    {
        CurrentSettings.UniqueForTargetFramework();
        return this;
    }

    /// <summary>
    /// Use the current test assembly TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    public SettingsTask UniqueForTargetFrameworkAndVersion()
    {
        CurrentSettings.UniqueForTargetFrameworkAndVersion();
        return this;
    }

    /// <summary>
    /// Use the current test assembly configuration (debug/release) to make the test results unique.
    /// Used when a test produces different results based on assembly configuration.
    /// </summary>
    public SettingsTask UniqueForAssemblyConfiguration()
    {
        CurrentSettings.UniqueForAssemblyConfiguration();
        return this;
    }
    
    /// <summary>
    /// Use <paramref name="assembly"/> TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    public SettingsTask UniqueForTargetFramework(Assembly assembly)
    {
        CurrentSettings.UniqueForTargetFramework(assembly);
        return this;
    }

    /// <summary>
    /// Use the <paramref name="assembly"/> TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    public SettingsTask UniqueForTargetFrameworkAndVersion(Assembly assembly)
    {
        CurrentSettings.UniqueForTargetFrameworkAndVersion(assembly);
        return this;
    }

    /// <summary>
    /// Use the <paramref name="assembly"/> configuration (debug/release) to make the test results unique.
    /// Used when a test produces different results based on assembly configuration.
    /// </summary>
    public SettingsTask UniqueForAssemblyConfiguration(Assembly assembly)
    {
        CurrentSettings.UniqueForAssemblyConfiguration(assembly);
        return this;
    }

    /// <summary>
    /// Allow multiple tests to map to the same snapshot file prefix.
    /// </summary>
    public SettingsTask DisableRequireUniquePrefix()
    {
        CurrentSettings.DisableRequireUniquePrefix();
        return this;
    }

    /// <summary>
    /// Use a custom method name for the test results.
    /// Where the file format is `{Directory}/{TestClassName}.{TestMethodName}_{Parameters}.{UniqueFor1}.{UniqueFor2}.{UniqueForX}.verified.{extension}`.
    /// </summary>
    /// <remarks>Not compatible with <see cref="UseFileName"/>.</remarks>
    public SettingsTask UseMethodName(string name)
    {
        CurrentSettings.UseMethodName(name);
        return this;
    }

    /// <summary>
    /// Use a custom directory for the test results.
    /// </summary>
    public SettingsTask UseDirectory(string directory)
    {
        CurrentSettings.UseDirectory(directory);
        return this;
    }

    /// <summary>
    /// Use a file name for the test results.
    /// Overrides the `{TestClassName}.{TestMethodName}_{Parameters}` parts of the file naming.
    /// Where the file format is `{Directory}/{TestClassName}.{TestMethodName}_{Parameters}.{UniqueFor1}.{UniqueFor2}.{UniqueForX}.verified.{extension}`.
    /// </summary>
    /// <remarks>Not compatible with <see cref="UseTypeName"/>, <see cref="UseMethodName"/>, or <see cref="UseParameters"/>.</remarks>
    public SettingsTask UseFileName(string fileName)
    {
        CurrentSettings.UseFileName(fileName);
        return this;
    }

    /// <summary>
    /// Use a custom class name for the test results.
    /// Where the file format is `{Directory}/{TestClassName}.{TestMethodName}_{Parameters}.{UniqueFor1}.{UniqueFor2}.{UniqueForX}.verified.{extension}`.
    /// </summary>
    /// <remarks>Not compatible with <see cref="UseFileName"/>.</remarks>
    public SettingsTask UseTypeName(string name)
    {
        CurrentSettings.UseTypeName(name);
        return this;
    }

    /// <summary>
    /// Use the current runtime and runtime version to make the test results unique.
    /// Used when a test produces different results based on runtime and runtime version.
    /// </summary>
    public SettingsTask UniqueForRuntimeAndVersion()
    {
        CurrentSettings.UniqueForRuntimeAndVersion();
        return this;
    }

    /// <summary>
    /// Use the current processor architecture (x86/x64/arm/arm64) to make the test results unique.
    /// Used when a test produces different results based on processor architecture.
    /// </summary>
    public SettingsTask UniqueForArchitecture()
    {
        CurrentSettings.UniqueForArchitecture();
        return this;
    }

    /// <summary>
    /// Use the operating system family (Linux/Windows/OSX) to make the test results unique.
    /// Used when a test produces different results based on operating system family.
    /// </summary>
    public SettingsTask UniqueForOSPlatform()
    {
        CurrentSettings.UniqueForOSPlatform();
        return this;
    }

    /// <summary>
    /// Remove the <see cref="Environment.MachineName"/> from the test results.
    /// </summary>
    public SettingsTask ScrubMachineName()
    {
        CurrentSettings.ScrubMachineName();
        return this;
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch"/> from the test results.
    /// </summary>
    public SettingsTask ScrubLinesContaining(StringComparison comparison, params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(comparison, stringToMatch);
        return this;
    }

    /// <summary>
    /// Remove any lines matching <paramref name="removeLine"/> from the test results.
    /// </summary>
    public SettingsTask ScrubLines(Func<string, bool> removeLine)
    {
        CurrentSettings.ScrubLines(removeLine);
        return this;
    }

    /// <summary>
    /// Scrub lines with an optional replace.
    /// <paramref name="replaceLine"/> can return the input to ignore the line, or return a a different string to replace it.
    /// </summary>
    public SettingsTask ScrubLinesWithReplace(Func<string, string?> replaceLine)
    {
        CurrentSettings.ScrubLinesWithReplace(replaceLine);
        return this;
    }

    /// <summary>
    /// Remove any lines containing only whitespace from the test results.
    /// </summary>
    public SettingsTask ScrubEmptyLines()
    {
        CurrentSettings.ScrubEmptyLines();
        return this;
    }

    /// <summary>
    /// Remove any lines containing any of <paramref name="stringToMatch"/> from the test results.
    /// </summary>
    public SettingsTask ScrubLinesContaining(params string[] stringToMatch)
    {
        CurrentSettings.ScrubLinesContaining(stringToMatch);
        return this;
    }

    public SettingsTask ModifySerialization(Action<SerializationSettings> action)
    {
        CurrentSettings.ModifySerialization(action);
        return this;
    }

    /// <summary>
    /// Automatically accept the results of the current test.
    /// </summary>
    public SettingsTask AutoVerify(bool includeBuildServer = true)
    {
        CurrentSettings.AutoVerify(includeBuildServer);
        return this;
    }

    /// <summary>
    /// Use a custom text for the `Parameters` part of the file name.
    /// Not compatible with <see cref="UseParameters"/>.
    /// Where the file format is `{Directory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    public SettingsTask UseTextForParameters(string parametersText)
    {
        CurrentSettings.UseTextForParameters(parametersText);
        return this;
    }

    /// <summary>
    /// Use a custom file extension for the test results.
    /// Where the file format is `{Directory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    public SettingsTask UseExtension(string extension)
    {
        CurrentSettings.UseExtension(extension);
        return this;
    }

    public VerifySettings CurrentSettings
    {
        get => settings ??= new();
    }

    public Task ToTask()
    {
        return task ??= buildTask(CurrentSettings);
    }

    public ConfiguredTaskAwaitable ConfigureAwait(bool continueOnCapturedContext)
    {
        return ToTask().ConfigureAwait(continueOnCapturedContext);
    }

    public TaskAwaiter GetAwaiter()
    {
        return ToTask().GetAwaiter();
    }

    public static implicit operator Task(SettingsTask settingsTask)
    {
        return settingsTask.ToTask();
    }
}
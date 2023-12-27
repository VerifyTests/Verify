namespace VerifyTests;

public partial class SettingsTask
{
    VerifySettings? settings;
    Func<VerifySettings, Task<VerifyResult>> buildTask;
    Task<VerifyResult>? task;

    public SettingsTask(VerifySettings? settings, Func<VerifySettings, Task<VerifyResult>> buildTask)
    {
        if (settings is not null)
        {
            this.settings = new(settings);
        }

        this.buildTask = buildTask;
    }

    [Pure]
    public SettingsTask AddExtraSettings(Action<JsonSerializerSettings> action)
    {
        CurrentSettings.AddExtraSettings(action);
        return this;
    }

    /// <summary>
    /// Append a key-value pair to the serialized target.
    /// </summary>
    [Pure]
    public SettingsTask AppendValue(string name, object data)
    {
        CurrentSettings.AppendValue(name, data);
        return this;
    }

    /// <summary>
    /// Append key-value pairs to the serialized target.
    /// </summary>
    [Pure]
    public SettingsTask AppendValues(IEnumerable<KeyValuePair<string, object>> values)
    {
        CurrentSettings.AppendValues(values);
        return this;
    }

    /// <summary>
    /// Append key-value pairs to the serialized target.
    /// </summary>
    [Pure]
    public SettingsTask AppendValues(params KeyValuePair<string, object>[] values)
    {
        CurrentSettings.AppendValues(values);
        return this;
    }

    /// <summary>
    /// Ignore parameters in 'verified' filename resulting in the same verified file for each testcase.
    /// Note that the 'received' files contain the parameters.
    /// </summary>
    [Pure]
    public SettingsTask IgnoreParametersForVerified(params object?[] parameters)
    {
        CurrentSettings.IgnoreParametersForVerified(parameters);
        return this;
    }

    /// <summary>
    /// Define the parameter values being used by a parameterised (aka data drive) test.
    /// In most cases the parameter parameter values can be automatically resolved.
    /// When this is not possible, an exception will be thrown instructing the use of <see cref="UseParameters" />
    /// Not compatible with <see cref="UseTextForParameters" />.
    /// </summary>
    [Pure]
    public SettingsTask UseParameters(params object?[] parameters)
    {
        CurrentSettings.UseParameters(parameters);
        return this;
    }

    [Pure]
    public SettingsTask UseParameters<T>(T parameter)
    {
        CurrentSettings.UseParameters(parameter);
        return this;
    }

    [Pure]
    public SettingsTask UseParameters<T>(T[] parameters)
    {
        CurrentSettings.UseParameters(parameters);
        return this;
    }

    [Pure]
    public SettingsTask UseStreamComparer(StreamCompare compare)
    {
        CurrentSettings.UseStreamComparer(compare);
        return this;
    }

    [Pure]
    public SettingsTask UseStringComparer(StringCompare compare)
    {
        CurrentSettings.UseStringComparer(compare);
        return this;
    }

    /// <summary>
    /// Disable using a diff toll for this test
    /// </summary>
    [Pure]
    public SettingsTask DisableDiff()
    {
        CurrentSettings.DisableDiff();
        return this;
    }

    /// <summary>
    /// Use the current runtime to make the test results unique.
    /// Used when a test produces different results based on runtime.
    /// </summary>
    [Pure]
    public SettingsTask UniqueForRuntime()
    {
        CurrentSettings.UniqueForRuntime();
        return this;
    }

    /// <summary>
    /// Use the current test assembly TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    [Pure]
    public SettingsTask UniqueForTargetFramework()
    {
        CurrentSettings.UniqueForTargetFramework();
        return this;
    }

    /// <summary>
    /// Use the current test assembly TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    [Pure]
    public SettingsTask UniqueForTargetFrameworkAndVersion()
    {
        CurrentSettings.UniqueForTargetFrameworkAndVersion();
        return this;
    }

    /// <summary>
    /// Use the current test assembly configuration (debug/release) to make the test results unique.
    /// Used when a test produces different results based on assembly configuration.
    /// </summary>
    [Pure]
    public SettingsTask UniqueForAssemblyConfiguration()
    {
        CurrentSettings.UniqueForAssemblyConfiguration();
        return this;
    }

    /// <summary>
    /// Use <paramref name="assembly" /> TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    [Pure]
    public SettingsTask UniqueForTargetFramework(Assembly assembly)
    {
        CurrentSettings.UniqueForTargetFramework(assembly);
        return this;
    }

    /// <summary>
    /// Use the <paramref name="assembly" /> TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    [Pure]
    public SettingsTask UniqueForTargetFrameworkAndVersion(Assembly assembly)
    {
        CurrentSettings.UniqueForTargetFrameworkAndVersion(assembly);
        return this;
    }

    /// <summary>
    /// Use the <paramref name="assembly" /> configuration (debug/release) to make the test results unique.
    /// Used when a test produces different results based on assembly configuration.
    /// </summary>
    [Pure]
    public SettingsTask UniqueForAssemblyConfiguration(Assembly assembly)
    {
        CurrentSettings.UniqueForAssemblyConfiguration(assembly);
        return this;
    }

    /// <summary>
    /// Allow multiple tests to map to the same snapshot file prefix.
    /// </summary>
    [Pure]
    public SettingsTask DisableRequireUniquePrefix()
    {
        CurrentSettings.DisableRequireUniquePrefix();
        return this;
    }

    /// <summary>
    /// Use a custom method name for the test results.
    /// Where the file format is `{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}.{UniqueFor1}.{UniqueFor2}.{UniqueForX}.verified.{extension}`.
    /// </summary>
    /// <remarks>Not compatible with <see cref="UseFileName" />.</remarks>
    [Pure]
    public SettingsTask UseMethodName(string name)
    {
        CurrentSettings.UseMethodName(name);
        return this;
    }

    /// <summary>
    /// Use a custom directory for the test results.
    /// </summary>
    [Pure]
    public SettingsTask UseDirectory(string directory)
    {
        CurrentSettings.UseDirectory(directory);
        return this;
    }

    /// <summary>
    /// Use a file name for the test results.
    /// Overrides the `{TestClassName}.{TestMethodName}_{Parameters}` parts of the file naming.
    /// Where the new file format is `{CurrentDirectory}/{FileName}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    /// <remarks>Not compatible with <see cref="UseTypeName" />, <see cref="UseMethodName" />, or <see cref="UseParameters" />.</remarks>
    [Pure]
    public SettingsTask UseFileName(string fileName)
    {
        CurrentSettings.UseFileName(fileName);
        return this;
    }

    /// <summary>
    /// Use a directory for the test results.
    /// Where the file format is `{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}/{targetName}.verified.{extension}`.
    /// </summary>
    [Pure]
    public SettingsTask UseUniqueDirectory()
    {
        CurrentSettings.UseUniqueDirectory();
        return this;
    }

    /// <summary>
    /// Use a custom class name for the CurrentDirectory results.
    /// Where the file format is `{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}.{UniqueFor1}.{UniqueFor2}.{UniqueForX}.verified.{extension}`.
    /// </summary>
    /// <remarks>Not compatible with <see cref="UseFileName" />.</remarks>
    [Pure]
    public SettingsTask UseTypeName(string name)
    {
        CurrentSettings.UseTypeName(name);
        return this;
    }

    /// <summary>
    /// Use the current runtime and runtime version to make the test results unique.
    /// Used when a test produces different results based on runtime and runtime version.
    /// </summary>
    [Pure]
    public SettingsTask UniqueForRuntimeAndVersion()
    {
        CurrentSettings.UniqueForRuntimeAndVersion();
        return this;
    }

    /// <summary>
    /// Provide parameters to hash together and pass to <see cref="UseTextForParameters" />.
    /// Used to get a deterministic file name while avoiding long paths.
    /// Combines <see cref="UseParameters" /> and <see cref="HashParameters" />.
    /// </summary>
    [Pure]
    public SettingsTask UseHashedParameters(params object?[] parameters)
    {
        CurrentSettings.UseHashedParameters(parameters);
        return this;
    }

    /// <summary>
    /// Hash parameters together and pass to <see cref="UseTextForParameters" />.
    /// Used to get a deterministic file name while avoiding long paths.
    /// </summary>
    [Pure]
    public SettingsTask HashParameters()
    {
        CurrentSettings.HashParameters();
        return this;
    }

    /// <summary>
    /// Use the current processor architecture (x86/x64/arm/arm64) to make the test results unique.
    /// Used when a test produces different results based on processor architecture.
    /// </summary>
    [Pure]
    public SettingsTask UniqueForArchitecture()
    {
        CurrentSettings.UniqueForArchitecture();
        return this;
    }

    /// <summary>
    /// Use the operating system family (Linux/Windows/OSX) to make the test results unique.
    /// Used when a test produces different results based on operating system family.
    /// </summary>
    [Pure]
    public SettingsTask UniqueForOSPlatform()
    {
        CurrentSettings.UniqueForOSPlatform();
        return this;
    }

    [Pure]
    public SettingsTask IgnoreStackTrace()
    {
        CurrentSettings.IgnoreStackTrace();
        return this;
    }

    /// <summary>
    /// Automatically accept the results of the current test.
    /// </summary>
    [Pure]
    public SettingsTask AutoVerify(bool includeBuildServer = true)
    {
        CurrentSettings.AutoVerify(includeBuildServer);
        return this;
    }

    /// <summary>
    /// Use a custom text for the `Parameters` part of the file name.
    /// Not compatible with <see cref="UseParameters" />.
    /// Where the file format is `{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    [Pure]
    public SettingsTask UseTextForParameters(string parametersText)
    {
        CurrentSettings.UseTextForParameters(parametersText);
        return this;
    }

    /// <summary>
    /// Use the current runtime to make the test results unique.
    /// Used when a test produces different results based on runtime.
    /// </summary>
    [Pure]
    public SettingsTask UseSplitModeForUniqueDirectory()
    {
        CurrentSettings.UseSplitModeForUniqueDirectory();
        return this;
    }

    /// <summary>
    /// Dont use the current runtime to make the test results unique.
    /// Overrides <see cref="VerifierSettings.UseSplitModeForUniqueDirectory" />.
    /// </summary>
    [Pure]
    public SettingsTask DontUseSplitModeForUniqueDirectory()
    {
        CurrentSettings.DontUseSplitModeForUniqueDirectory();
        return this;
    }

    public VerifySettings CurrentSettings => settings ??= new();

    [Pure]
    public Task<VerifyResult> ToTask() =>
        task ??= buildTask(CurrentSettings);

    [Pure]
    public ConfiguredTaskAwaitable<VerifyResult> ConfigureAwait(bool continueOnCapturedContext) =>
        ToTask()
            .ConfigureAwait(continueOnCapturedContext);

    [Pure]
    public TaskAwaiter<VerifyResult> GetAwaiter() =>
        ToTask()
            .GetAwaiter();

    [Pure]
    public static implicit operator Task<VerifyResult>(SettingsTask settingsTask) =>
        settingsTask.ToTask();
}
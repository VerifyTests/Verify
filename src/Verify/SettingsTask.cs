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

    public SettingsTask AddExtraSettings(Action<JsonSerializerSettings> action)
    {
        CurrentSettings.AddExtraSettings(action);
        return this;
    }

    /// <summary>
    /// Append a key-value pair to the serialized target.
    /// </summary>
    public SettingsTask AppendValue(string name, object data)
    {
        CurrentSettings.AppendValue(name, data);
        return this;
    }

    /// <summary>
    /// Append key-value pairs to the serialized target.
    /// </summary>
    public SettingsTask AppendValues(IEnumerable<KeyValuePair<string, object>> values)
    {
        CurrentSettings.AppendValues(values);
        return this;
    }

    /// <summary>
    /// Append key-value pairs to the serialized target.
    /// </summary>
    public SettingsTask AppendValues(params KeyValuePair<string, object>[] values)
    {
        CurrentSettings.AppendValues(values);
        return this;
    }

    /// <summary>
    /// Ignore parameters in 'verified' filename resulting in the same verified file for each testcase.
    /// Note that the 'received' files contain the parameters.
    /// </summary>
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
    public SettingsTask UseParameters(params object?[] parameters)
    {
        CurrentSettings.UseParameters(parameters);
        return this;
    }

    public SettingsTask UseParameters<T>(T parameter)
    {
        CurrentSettings.UseParameters(parameter);
        return this;
    }

    public SettingsTask UseParameters<T>(T[] parameters)
    {
        CurrentSettings.UseParameters(parameters);
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
    /// Use <paramref name="assembly" /> TargetFrameworkAttribute to make the test results unique.
    /// Used when a test produces different results based on TargetFramework.
    /// </summary>
    public SettingsTask UniqueForTargetFramework(Assembly assembly)
    {
        CurrentSettings.UniqueForTargetFramework(assembly);
        return this;
    }

    /// <summary>
    /// Use the <paramref name="assembly" /> TargetFrameworkAttribute name and version to make the test results unique.
    /// Used when a test produces different results based on TargetFramework and TargetFramework version.
    /// </summary>
    public SettingsTask UniqueForTargetFrameworkAndVersion(Assembly assembly)
    {
        CurrentSettings.UniqueForTargetFrameworkAndVersion(assembly);
        return this;
    }

    /// <summary>
    /// Use the <paramref name="assembly" /> configuration (debug/release) to make the test results unique.
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
    /// Where the file format is `{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}.{UniqueFor1}.{UniqueFor2}.{UniqueForX}.verified.{extension}`.
    /// </summary>
    /// <remarks>Not compatible with <see cref="UseFileName" />.</remarks>
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
    /// Where the new file format is `{CurrentDirectory}/{FileName}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    /// <remarks>Not compatible with <see cref="UseTypeName" />, <see cref="UseMethodName" />, or <see cref="UseParameters" />.</remarks>
    public SettingsTask UseFileName(string fileName)
    {
        CurrentSettings.UseFileName(fileName);
        return this;
    }

    /// <summary>
    /// Use a directory for the test results.
    /// Where the file format is `{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}/{targetName}.verified.{extension}`.
    /// </summary>
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
    /// Hash parameters together to and pass to <see cref="UseTextForParameters"/>.
    /// Used to get a deterministic file name while avoiding long paths.
    /// </summary>
    public SettingsTask UseParametersHash(params object?[] parameters)
    {
        CurrentSettings.UseParametersHash(parameters);
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

    public SettingsTask IgnoreStackTrace()
    {
        CurrentSettings.IgnoreStackTrace();
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
    /// Not compatible with <see cref="UseParameters" />.
    /// Where the file format is `{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    public SettingsTask UseTextForParameters(string parametersText)
    {
        CurrentSettings.UseTextForParameters(parametersText);
        return this;
    }

    /// <summary>
    /// Use the current runtime to make the test results unique.
    /// Used when a test produces different results based on runtime.
    /// </summary>
    public SettingsTask UseSplitModeForUniqueDirectory()
    {
        CurrentSettings.UseSplitModeForUniqueDirectory();
        return this;
    }

    /// <summary>
    /// Dont use the current runtime to make the test results unique.
    /// Overrides <see cref="VerifierSettings.UseSplitModeForUniqueDirectory"/>.
    /// </summary>
    public SettingsTask DontUseSplitModeForUniqueDirectory()
    {
        CurrentSettings.DontUseSplitModeForUniqueDirectory();
        return this;
    }

    public VerifySettings CurrentSettings => settings ??= new();

    public Task<VerifyResult> ToTask() =>
        task ??= buildTask(CurrentSettings);

    public ConfiguredTaskAwaitable<VerifyResult> ConfigureAwait(bool continueOnCapturedContext) =>
        ToTask().ConfigureAwait(continueOnCapturedContext);

    public TaskAwaiter<VerifyResult> GetAwaiter() =>
        ToTask().GetAwaiter();

    public static implicit operator Task<VerifyResult>(SettingsTask settingsTask) =>
        settingsTask.ToTask();
}
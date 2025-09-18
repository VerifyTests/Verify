namespace VerifyTests;

public partial class SettingsTask
{
    Func<VerifySettings, Task<VerifyResult>> buildTask;
    Task<VerifyResult>? task;

    public SettingsTask(VerifySettings? settings, Func<VerifySettings, Task<VerifyResult>> buildTask)
    {
        if (settings is not null)
        {
            CurrentSettings = new(settings);
        }

        this.buildTask = buildTask;
    }

    /// <inheritdoc cref="VerifySettings.AddExtraSettings"/>
    [Pure]
    public SettingsTask AddExtraSettings(Action<JsonSerializerSettings> action)
    {
        CurrentSettings.AddExtraSettings(action);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AppendValue(string,object)"/>
    [Pure]
    public SettingsTask AppendValue(string name, object data)
    {
        CurrentSettings.AppendValue(name, data);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AppendValue(string,object)"/>
    [Pure]
    [OverloadResolutionPriority(-1)]
    public SettingsTask AppendValue(object data, [CallerArgumentExpression(nameof(data))] string? name = null)
    {
        CurrentSettings.AppendValue(name!, data);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AppendValues(IEnumerable{KeyValuePair{string,object}})"/>
    [Pure]
    public SettingsTask AppendValues(IEnumerable<KeyValuePair<string, object>> values)
    {
        CurrentSettings.AppendValues(values);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AppendValues(IEnumerable{KeyValuePair{string,object}})"/>
    [Pure]
    public SettingsTask AppendValues(params KeyValuePair<string, object>[] values)
    {
        CurrentSettings.AppendValues(values);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UseStreamComparer(StreamCompare)"/>
    [Pure]
    public SettingsTask UseStreamComparer(StreamCompare compare)
    {
        CurrentSettings.UseStreamComparer(compare);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UseStringComparer(StringCompare)"/>
    [Pure]
    public SettingsTask UseStringComparer(StringCompare compare)
    {
        CurrentSettings.UseStringComparer(compare);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.DisableDiff()"/>
    [Pure]
    public SettingsTask DisableDiff()
    {
        CurrentSettings.DisableDiff();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UniqueForRuntime()"/>
    [Pure]
    public SettingsTask UniqueForRuntime()
    {
        CurrentSettings.UniqueForRuntime();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UniqueForTargetFramework()"/>
    [Pure]
    public SettingsTask UniqueForTargetFramework()
    {
        CurrentSettings.UniqueForTargetFramework();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UniqueForTargetFrameworkAndVersion()"/>
    [Pure]
    public SettingsTask UniqueForTargetFrameworkAndVersion()
    {
        CurrentSettings.UniqueForTargetFrameworkAndVersion();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UniqueForAssemblyConfiguration()"/>
    [Pure]
    public SettingsTask UniqueForAssemblyConfiguration()
    {
        CurrentSettings.UniqueForAssemblyConfiguration();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UniqueForTargetFramework(Assembly)"/>
    [Pure]
    public SettingsTask UniqueForTargetFramework(Assembly assembly)
    {
        CurrentSettings.UniqueForTargetFramework(assembly);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UniqueForTargetFrameworkAndVersion(Assembly)"/>
    [Pure]
    public SettingsTask UniqueForTargetFrameworkAndVersion(Assembly assembly)
    {
        CurrentSettings.UniqueForTargetFrameworkAndVersion(assembly);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UniqueForAssemblyConfiguration(Assembly)"/>
    [Pure]
    public SettingsTask UniqueForAssemblyConfiguration(Assembly assembly)
    {
        CurrentSettings.UniqueForAssemblyConfiguration(assembly);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.DisableRequireUniquePrefix()"/>
    [Pure]
    public SettingsTask DisableRequireUniquePrefix()
    {
        CurrentSettings.DisableRequireUniquePrefix();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UseMethodName(string)"/>
    [Pure]
    public SettingsTask UseMethodName(string name)
    {
        CurrentSettings.UseMethodName(name);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UseDirectory(string)"/>
    [Pure]
    public SettingsTask UseDirectory(string directory)
    {
        CurrentSettings.UseDirectory(directory);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UseFileName(string)"/>
    [Pure]
    public SettingsTask UseFileName(string fileName)
    {
        CurrentSettings.UseFileName(fileName);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UseUniqueDirectory()"/>
    [Pure]
    public SettingsTask UseUniqueDirectory()
    {
        CurrentSettings.UseUniqueDirectory();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UseTypeName(string)"/>
    [Pure]
    public SettingsTask UseTypeName(string name)
    {
        CurrentSettings.UseTypeName(name);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UniqueForRuntimeAndVersion()"/>
    [Pure]
    public SettingsTask UniqueForRuntimeAndVersion()
    {
        CurrentSettings.UniqueForRuntimeAndVersion();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UniqueForArchitecture()"/>
    [Pure]
    public SettingsTask UniqueForArchitecture()
    {
        CurrentSettings.UniqueForArchitecture();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UniqueForOSPlatform()"/>
    [Pure]
    public SettingsTask UniqueForOSPlatform()
    {
        CurrentSettings.UniqueForOSPlatform();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreStackTrace()"/>
    [Pure]
    public SettingsTask IgnoreStackTrace()
    {
        CurrentSettings.IgnoreStackTrace();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AutoVerify(bool)"/>
    [Obsolete("Use SettingsTask.AutoVerify(bool includeBuildServer, bool throwException)")]
    [Pure]
    public SettingsTask AutoVerify(bool includeBuildServer = true)
    {
        CurrentSettings.AutoVerify(includeBuildServer, false);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AutoVerify(bool, bool)"/>
    [Pure]
    [OverloadResolutionPriority(1)]
    public SettingsTask AutoVerify(bool includeBuildServer = true, bool throwException = false)
    {
        CurrentSettings.AutoVerify(includeBuildServer, throwException);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AutoVerify(VerifyTests.AutoVerify, bool)"/>
    [Obsolete("Use SettingsTask.AutoVerify(AutoVerify, autoVerify, bool includeBuildServer, bool throwException)")]
    [Pure]
    public SettingsTask AutoVerify(AutoVerify autoVerify, bool includeBuildServer = true)
    {
        CurrentSettings.AutoVerify(autoVerify, includeBuildServer, false);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.AutoVerify(VerifyTests.AutoVerify, bool, bool)"/>
    [Pure]
    [OverloadResolutionPriority(1)]
    public SettingsTask AutoVerify(AutoVerify autoVerify, bool includeBuildServer = true, bool throwException = false)
    {
        CurrentSettings.AutoVerify(autoVerify, includeBuildServer, throwException);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UseSplitModeForUniqueDirectory()"/>
    [Pure]
    public SettingsTask UseSplitModeForUniqueDirectory()
    {
        CurrentSettings.UseSplitModeForUniqueDirectory();
        return this;
    }

    /// <inheritdoc cref="VerifySettings.DontUseSplitModeForUniqueDirectory()"/>
    [Pure]
    public SettingsTask DontUseSplitModeForUniqueDirectory()
    {
        CurrentSettings.DontUseSplitModeForUniqueDirectory();
        return this;
    }

    [field: AllowNull, MaybeNull]
    public VerifySettings CurrentSettings
    {
        get
        {
            if (task == null)
            {
                return field ??= new();
            }

            throw new("This SettingsTask instance has already been converted to a Task and can no longer be modified. Conversion to a Task occurs either through awaiting the instance or calling ToTask.");
        }
    }

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
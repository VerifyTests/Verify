namespace VerifyTests;

public partial class VerifySettings
{
    internal string TxtOrJson
    {
        get
        {
            if (VerifierSettings.StrictJson || strictJson)
            {
                return "json";
            }

            return "txt";
        }
    }

    public VerifySettings(VerifySettings? settings)
    {
        if (settings is null)
        {
            return;
        }

        ScrubbersEnabled = settings.ScrubbersEnabled;
        dateCountingEnable = settings.dateCountingEnable;
        InstanceScrubbers = [..settings.InstanceScrubbers];
        ExtensionMappedInstanceScrubbers = new(settings.ExtensionMappedInstanceScrubbers);
        diffEnabled = settings.diffEnabled;
        MethodName = settings.MethodName;
        TypeName = settings.TypeName;
        strictJson = settings.strictJson;
        appendedFiles = settings.appendedFiles;
        UniqueDirectory = settings.UniqueDirectory;
        Directory = settings.Directory;
        autoVerify = settings.autoVerify;
        serialization = settings.serialization;
        stringComparer = settings.stringComparer;
        streamComparer = settings.streamComparer;
        extensionStringComparers = settings.extensionStringComparers;
        extensionStreamComparers = settings.extensionStreamComparers;
        parameters = settings.parameters;
        ignoredParameters = settings.ignoredParameters;
        ignoreParametersForVerified = settings.ignoreParametersForVerified;
        parametersText = settings.parametersText;
        FileName = settings.FileName;
        handleOnFirstVerify = settings.handleOnFirstVerify;
        handleOnVerifyDelete = settings.handleOnVerifyDelete;
        handleOnVerifyMismatch = settings.handleOnVerifyMismatch;
        UniquePrefixDisabled = settings.UniquePrefixDisabled;
        ParametersAppender = settings.ParametersAppender;
        beforeVerify = settings.beforeVerify;
        afterVerify = settings.afterVerify;
        UseUniqueDirectorySplitMode = settings.UseUniqueDirectorySplitMode;
        Namer = new(settings.Namer);
#if NET6_0_OR_GREATER
        namedDates = new(settings.namedDates);
        namedTimes = new(settings.namedTimes);
#endif
        namedGuids = new(settings.namedGuids);
        namedDateTimes = new(settings.namedDateTimes);
        namedDateTimeOffsets = new(settings.namedDateTimeOffsets);
        foreach (var append in settings.Appends)
        {
            if (append.Data is ICloneable cloneable)
            {
                AppendValue(append.Name, cloneable.Clone());
            }
            else
            {
                AppendValue(append.Name, append.Data);
            }
        }

        foreach (var pair in settings.Context)
        {
            if (pair.Value is ICloneable cloneable)
            {
                Context.Add(pair.Key, cloneable.Clone());
            }
            else
            {
                Context.Add(pair.Key, pair.Value);
            }
        }
    }

    /// <summary>
    /// Allows extensions to Verify to pass config via <see cref="VerifySettings" />.
    /// </summary>
    public Dictionary<string, object> Context { get; } = [];

    public VerifySettings()
    {
    }

    internal string? parametersText;

    /// <summary>
    /// Use a custom text for the `Parameters` part of the file name.
    /// Not compatible with <see cref="UseParameters" />.
    /// Where the file format is `{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    public void UseTextForParameters(string parametersText)
    {
        Guards.AgainstBadExtension(parametersText);

        if (parameters is not null)
        {
            throw new($"{nameof(UseTextForParameters)} is not compatible with {nameof(UseParameters)}.");
        }

        this.parametersText = parametersText;
    }

    internal AutoVerify? autoVerify;
    internal bool throwException;

    /// <summary>
    /// Automatically accept the results of the current test.
    /// </summary>
    [Obsolete("Use VerifySettings.AutoVerify(bool includeBuildServer, bool throwException)")]
    public void AutoVerify(bool includeBuildServer = true) =>
        AutoVerify(includeBuildServer, false);

    /// <summary>
    /// Automatically accept the results of the current test.
    /// </summary>
    [OverloadResolutionPriority(1)]
    public void AutoVerify(bool includeBuildServer = true, bool throwException = false) =>
        AutoVerify(_ => true, includeBuildServer, throwException);

    /// <summary>
    /// Automatically accept the results of the current test.
    /// </summary>
    [Obsolete("Use VerifySettings.AutoVerify(AutoVerify, autoVerify, bool includeBuildServer, bool throwException)")]
    public void AutoVerify(AutoVerify autoVerify, bool includeBuildServer = true) =>
        AutoVerify(autoVerify, includeBuildServer, false);

    /// <summary>
    /// Automatically accept the results of the current test.
    /// </summary>
    [OverloadResolutionPriority(1)]
    public void AutoVerify(AutoVerify autoVerify, bool includeBuildServer = true, bool throwException = false)
    {
        if (includeBuildServer ||
            !BuildServerDetector.Detected)
        {
           this.autoVerify = autoVerify;
           this.throwException = throwException;
        }
    }
}
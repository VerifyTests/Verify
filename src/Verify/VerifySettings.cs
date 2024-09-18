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

        dateCountingEnable = settings.dateCountingEnable;
        InstanceScrubbers = [..settings.InstanceScrubbers];
        ExtensionMappedInstanceScrubbers = new(settings.ExtensionMappedInstanceScrubbers);
        diffEnabled = settings.diffEnabled;
        methodName = settings.methodName;
        typeName = settings.typeName;
        strictJson = settings.strictJson;
        appendedFiles = settings.appendedFiles;
        useUniqueDirectory = settings.useUniqueDirectory;
        Directory = settings.Directory;
        autoVerify = settings.autoVerify;
        serialization = settings.serialization;
        stringComparer = settings.stringComparer;
        streamComparer = settings.streamComparer;
        parameters = settings.parameters;
        ignoredParameters = settings.ignoredParameters;
        ignoreParametersForVerified = settings.ignoreParametersForVerified;
        hashParameters = settings.hashParameters;
        parametersText = settings.parametersText;
        fileName = settings.fileName;
        UniquePrefixDisabled = settings.UniquePrefixDisabled;
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

    /// <summary>
    /// Automatically accept the results of the current test.
    /// </summary>
    public void AutoVerify(bool includeBuildServer = true) =>
        AutoVerify(_ => true, includeBuildServer);

    /// <summary>
    /// Automatically accept the results of the current test.
    /// </summary>
    public void AutoVerify(AutoVerify autoVerify, bool includeBuildServer = true)
    {
        if (includeBuildServer)
        {
           this.autoVerify = autoVerify;
        }
        else
        {
            if (!BuildServerDetector.Detected)
            {
                this.autoVerify = autoVerify;
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() =>
        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        base.GetHashCode();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string? ToString() =>
        base.ToString();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) =>
        // ReSharper disable once BaseObjectEqualsIsObjectEquals
        base.Equals(obj);
}
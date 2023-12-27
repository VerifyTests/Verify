namespace VerifyTests;

public partial class VerifySettings
{
    public VerifySettings(VerifySettings? settings)
    {
        if (settings is null)
        {
            return;
        }

        dateCountingEnable = settings.dateCountingEnable;
        InstanceScrubbers = [..settings.InstanceScrubbers];
        ExtensionMappedInstanceScrubbers = new(settings.ExtensionMappedInstanceScrubbers);
#if DiffEngine
        diffEnabled = settings.diffEnabled;
#endif
        methodName = settings.methodName;
        typeName = settings.typeName;
        appendedFiles = settings.appendedFiles;
        useUniqueDirectory = settings.useUniqueDirectory;
        Directory = settings.Directory;
        autoVerify = settings.autoVerify;
        serialization = settings.serialization;
        stringComparer = settings.stringComparer;
        streamComparer = settings.streamComparer;
        parameters = settings.parameters;
        ignoreParametersForVerified = settings.ignoreParametersForVerified;
        hashParameters = settings.hashParameters;
        parametersText = settings.parametersText;
        fileName = settings.fileName;
        UniquePrefixDisabled = settings.UniquePrefixDisabled;
        UseUniqueDirectorySplitMode = settings.UseUniqueDirectorySplitMode;
        Namer = new(settings.Namer);
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
        Guard.AgainstBadExtension(parametersText);

        if (parameters is not null)
        {
            throw new($"{nameof(UseTextForParameters)} is not compatible with {nameof(UseParameters)}.");
        }

        this.parametersText = parametersText;
    }

    internal bool IsAutoVerify =>
        VerifierSettings.autoVerify ||
        autoVerify;

    bool autoVerify;

    /// <summary>
    /// Automatically accept the results of the current test.
    /// </summary>
    // ReSharper disable once UnusedParameter.Global
    // ReSharper disable once MemberCanBeMadeStatic.Global
    public void AutoVerify(bool includeBuildServer = true)
    {
#if DiffEngine
        if (includeBuildServer)
        {
            autoVerify = true;
        }
        else
        {
            if (!BuildServerDetector.Detected)
            {
                autoVerify = true;
            }
        }
#endif
    }
}
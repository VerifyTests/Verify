using DiffEngine;

namespace VerifyTests;

public partial class VerifySettings
{
    public VerifySettings(VerifySettings? settings)
    {
        if (settings is null)
        {
            return;
        }

        instanceScrubbers = new(settings.instanceScrubbers);
        extensionMappedInstanceScrubbers = new(settings.extensionMappedInstanceScrubbers);
        extension = settings.extension;
        diffEnabled = settings.diffEnabled;
        methodName = settings.methodName;
        typeName = settings.typeName;
        Directory = settings.Directory;
        autoVerify = settings.autoVerify;
        serialization = settings.serialization;
        stringComparer = settings.stringComparer;
        streamComparer = settings.streamComparer;
        parameters = settings.parameters;
        parametersText = settings.parametersText;
        fileName = settings.fileName;
        UniquePrefixDisabled = settings.UniquePrefixDisabled;
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
    /// Allows extensions to Verify to pass config via <see cref="VerifySettings"/>.
    /// </summary>
    public Dictionary<string, object> Context { get; } = new();

    public VerifySettings()
    {
    }

    internal string? parametersText;

    /// <summary>
    /// Use a custom text for the `Parameters` part of the file name.
    /// Not compatible with <see cref="UseParameters"/>.
    /// Where the file format is `{Directory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    public void UseTextForParameters(string parametersText)
    {
        Guard.AgainstBadExtension(parametersText, nameof(parametersText));

        if (parameters is not null)
        {
            throw new($"{nameof(UseTextForParameters)} is not compatible with {nameof(UseParameters)}.");
        }

        this.parametersText = parametersText;
    }

    internal string? extension;

    /// <summary>
    /// Use a custom file extension for the test results.
    /// Where the file format is `{Directory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    public void UseExtension(string extension)
    {
        Guard.AgainstBadExtension(extension, nameof(extension));
        this.extension = extension;
    }

    /// <summary>
    /// Retrieves the value passed into <see cref="UseExtension"/>, if it exists.
    /// </summary>
    public bool TryGetExtension([NotNullWhen(true)] out string? extension)
    {
        if (this.extension is null)
        {
            extension = null;
            return false;
        }

        extension = this.extension;
        return true;
    }

    internal string ExtensionOrTxt(string defaultValue = "txt")
    {
        return extension ?? defaultValue;
    }

    internal string ExtensionOrBin()
    {
        return extension ?? "bin";
    }

    internal bool autoVerify;

    /// <summary>
    /// Automatically accept the results of the current test.
    /// </summary>
    public void AutoVerify(bool includeBuildServer = true)
    {
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
    }
}
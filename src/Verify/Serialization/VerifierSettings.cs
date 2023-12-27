// ReSharper disable RedundantSuppressNullableWarningExpression

// ReSharper disable UnusedParameter.Local

namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static SerializationSettings serialization = new();

    public static bool TryGetToString(
        object target,
        [NotNullWhen(true)] out Func<object, IReadOnlyDictionary<string, object>, AsStringResult>? toString)
    {
        if (target is Encoding encoding)
        {
            toString = (_, _) => encoding.EncodingName;
            return true;
        }

        if (target is Expression expression)
        {
            toString = (_, _) => expression.ToString();
            return true;
        }

        if (TypeNameConverter.TryGetSimpleName(target, out var name))
        {
            toString = (_, _) => name;
            return true;
        }

        return typeToString.TryGetValue(target.GetType(), out toString);
    }

    static Dictionary<Type, Func<object, IReadOnlyDictionary<string, object>, AsStringResult>> typeToString = new()
    {
        #region typeToStringMapping

        {
            typeof(StringBuilder), (target, _) => ((StringBuilder) target).ToString()
        },
        {
            typeof(StringWriter), (target, _) => ((StringWriter) target).ToString()
        },
        {
            typeof(bool), (target, _) => ((bool) target).ToString(Culture.InvariantCulture)
        },
        {
            typeof(short), (target, _) => ((short) target).ToString(Culture.InvariantCulture)
        },
        {
            typeof(ushort), (target, _) => ((ushort) target).ToString(Culture.InvariantCulture)
        },
        {
            typeof(int), (target, _) => ((int) target).ToString(Culture.InvariantCulture)
        },
        {
            typeof(uint), (target, _) => ((uint) target).ToString(Culture.InvariantCulture)
        },
        {
            typeof(long), (target, _) => ((long) target).ToString(Culture.InvariantCulture)
        },
        {
            typeof(ulong), (target, _) => ((ulong) target).ToString(Culture.InvariantCulture)
        },
        {
            typeof(decimal), (target, _) => ((decimal) target).ToString(Culture.InvariantCulture)
        },
        {
            typeof(BigInteger), (target, _) => ((BigInteger) target).ToString(Culture.InvariantCulture)
        },
#if NET5_0_OR_GREATER
        {
            typeof(Half), (target, _) => ((Half) target).ToString(Culture.InvariantCulture)
        },
#endif
#if NET6_0_OR_GREATER
        {
            typeof(Date), (target, _) =>
            {
                var date = (Date) target;
                return date.ToString("yyyy-MM-dd", Culture.InvariantCulture);
            }
        },
        {
            typeof(Time), (target, _) =>
            {
                var time = (Time) target;
                return time.ToString("h:mm tt", Culture.InvariantCulture);
            }
        },
#endif
        {
            typeof(float), (target, _) => ((float) target).ToString(Culture.InvariantCulture)
        },
        {
            typeof(double), (target, _) => ((double) target).ToString(Culture.InvariantCulture)
        },
        {
            typeof(Guid), (target, _) => ((Guid) target).ToString()
        },
        {
            typeof(DateTime), (target, _) => DateFormatter.ToJsonString((DateTime) target)
        },
        {
            typeof(DateTimeOffset), (target, _) => DateFormatter.ToJsonString((DateTimeOffset) target)
        },
        {
            typeof(XmlNode), (target, _) =>
            {
                var converted = (XmlNode) target;
                var document = XDocument.Parse(converted.OuterXml);
                return new(document.ToString(), "xml");
            }
        },
        {
            typeof(XElement), (target, settings) =>
            {
                var converted = (XElement) target;
                return new(converted.ToString(), "xml");
            }
        },

        #endregion
    };

    public static void TreatAsString<T>(AsString<T>? toString = null)
        where T : notnull
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        toString ??= (target, _) => new(target.ToString()!);
        typeToString[typeof(T)] = (target, settings) => toString((T) target, settings);
    }

    internal static void Reset()
    {
        InnerVerifier.verifyHasBeenRun = false;
        DateCountingEnabled = true;
        StrictJson = false;
        scrubProjectDir = true;
        scrubSolutionDir = true;
        sortPropertiesAlphabetically = false;
        sortJsonObjects = false;
        autoVerify = false;
        UniquePrefixDisabled = false;
        UseUniqueDirectorySplitMode = false;
        omitContentFromException = false;
        encoding = new UTF8Encoding(true, true);
        GlobalScrubbers.Clear();
    }

    public static void UseStrictJson()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        StrictJson = true;
    }

    public static bool StrictJson { get; private set; }

    internal static string TxtOrJson
    {
        get
        {
            if (StrictJson)
            {
                return "json";
            }

            return "txt";
        }
    }

    internal static bool scrubProjectDir = true;

    public static void DontScrubProjectDirectory()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        scrubProjectDir = false;
    }

    internal static bool scrubSolutionDir = true;

    public static void DontScrubSolutionDirectory()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        scrubSolutionDir = false;
    }

    internal static bool sortPropertiesAlphabetically;

    public static void SortPropertiesAlphabetically()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        sortPropertiesAlphabetically = true;
    }

    internal static bool sortJsonObjects;

    public static void SortJsonObjects()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        sortJsonObjects = true;
    }
}
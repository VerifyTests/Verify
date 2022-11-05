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

        return typeToString.TryGetValue(target!.GetType(), out toString);
    }

    static Dictionary<Type, Func<object, IReadOnlyDictionary<string, object>, AsStringResult>> typeToString = new()
    {
        #region typeToStringMapping
        {typeof(StringBuilder), (target, _) => ((StringBuilder) target).ToString()},
        {typeof(StringWriter), (target, _) => ((StringWriter) target).ToString()},
        {typeof(bool), (target, _) => ((bool) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(short), (target, _) => ((short) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(ushort), (target, _) => ((ushort) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(int), (target, _) => ((int) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(uint), (target, _) => ((uint) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(long), (target, _) => ((long) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(ulong), (target, _) => ((ulong) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(decimal), (target, _) => ((decimal) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(BigInteger), (target, _) => ((BigInteger) target).ToString(CultureInfo.InvariantCulture)},
#if NET5_0_OR_GREATER
        {typeof(Half), (target, _) => ((Half) target).ToString(CultureInfo.InvariantCulture)},
#endif
#if NET6_0_OR_GREATER
        {
            typeof(DateOnly), (target, _) =>
            {
                var date = (DateOnly) target;
                return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
        },
        {
            typeof(TimeOnly), (target, _) =>
            {
                var time = (TimeOnly) target;
                return time.ToString("h:mm tt", CultureInfo.InvariantCulture);
            }
        },
#endif
        {typeof(float), (target, _) => ((float) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(double), (target, _) => ((double) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(Guid), (target, _) => ((Guid) target).ToString()},
        {typeof(DateTime), (target, _) => DateFormatter.ToJsonString((DateTime) target)},
        {typeof(DateTimeOffset), (target, _) => DateFormatter.ToJsonString((DateTimeOffset) target)},
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
        toString ??= (target, _) => new(target.ToString()!);
        typeToString[typeof(T)] = (target, settings) => toString((T) target, settings);
    }

    public static void UseStrictJson() =>
        StrictJson = true;

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

    public static void DontScrubProjectDirectory() =>
        scrubProjectDir = false;

    internal static bool scrubSolutionDir = true;

    public static void DontScrubSolutionDirectory() =>
        scrubSolutionDir = false;

    internal static bool sortPropertiesAlphabetically;

    public static void SortPropertiesAlphabetically() =>
        sortPropertiesAlphabetically = true;

    internal static bool sortJsonObjects;

    public static void SortJsonObjects() =>
        sortJsonObjects = true;
}
using System.Xml;
using System.Xml.Linq;

// ReSharper disable UnusedParameter.Local

namespace VerifyTests;

public static partial class VerifierSettings
{
    internal static SerializationSettings serialization = new();

    public static bool TryGetToString<T>(
        T target,
        [NotNullWhen(true)] out Func<object, IReadOnlyDictionary<string, object>, AsStringResult>? toString)
    {
        if (target is Type type)
        {
            toString = (_, _) => type.SimpleName();
            return true;
        }

        if (target is FieldInfo field)
        {
            toString = (_, _) => field.SimpleName();
            return true;
        }

        if (target is PropertyInfo property)
        {
            toString = (_, _) => property.SimpleName();
            return true;
        }

        if (target is MethodInfo method)
        {
            toString = (_, _) => method.SimpleName();
            return true;
        }

        if (target is ConstructorInfo constructor)
        {
            toString = (_, _) => constructor.SimpleName();
            return true;
        }

        if (target is ParameterInfo parameter)
        {
            toString = (_, _) => parameter.SimpleName();
            return true;
        }

        return typeToString.TryGetValue(target!.GetType(), out toString);
    }

    static Dictionary<Type, Func<object, IReadOnlyDictionary<string, object>, AsStringResult>> typeToString = new()
    {
        #region typeToStringMapping

        {typeof(string), (target, _) => (string) target},
        {typeof(StringBuilder), (target, _) => ((StringBuilder) target).ToString()},
        {typeof(bool), (target, _) => ((bool) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(short), (target, _) => ((short) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(ushort), (target, _) => ((ushort) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(int), (target, _) => ((int) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(uint), (target, _) => ((uint) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(long), (target, _) => ((long) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(ulong), (target, _) => ((ulong) target).ToString(CultureInfo.InvariantCulture)},
        {typeof(decimal), (target, _) => ((decimal) target).ToString(CultureInfo.InvariantCulture)},
#if NET5_0_OR_GREATER
        {typeof(Half), (target, settings) => ((Half) target).ToString(CultureInfo.InvariantCulture)},
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
        {
            typeof(DateTime), (target, _) =>
            {
                var dateTime = (DateTime) target;
                return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFz");
            }
        },
        {
            typeof(DateTimeOffset), (target, _) =>
            {
                var dateTimeOffset = (DateTimeOffset) target;
                return dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFz", CultureInfo.InvariantCulture);
            }
        },
        {
            typeof(XmlNode), (target, settings) =>
            {
                var converted = (XmlNode) target;
                var document = XDocument.Parse(converted.OuterXml);
                return new(document.ToString(), "xml");
            }
        },
        {
            typeof(XDocument), (target, settings) =>
            {
                var converted = (XDocument) target;
                return new(converted.ToString(), "xml");
            }
        },
        {
            typeof(XElement), (target, settings) =>
            {
                var converted = (XElement) target;
                return new(converted.ToString(), "xml");
            }
        },
        {
            typeof(XmlDocument), (target, settings) =>
            {
                var xmlDocument = (XmlDocument) target;
                var stringBuilder = new StringBuilder();
                var writerSettings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };
                using (var writer = XmlWriter.Create(stringBuilder, writerSettings))
                {
                    xmlDocument.Save(writer);
                }
                return new(stringBuilder.ToString(), "xml");
            }
        }

        #endregion
    };

    public static void TreatAsString<T>(AsString<T>? toString = null)
    {
        toString ??= (target, _) =>
        {
            if (target is null)
            {
                return new("null");
            }

            return new(target.ToString()!);
        };
        typeToString[typeof(T)] = (target, settings) => toString((T) target, settings);
    }

    public static void AddExtraSettings(Action<JsonSerializerSettings> action)
    {
        serialization.AddExtraSettings(action);
    }

    public static void ModifySerialization(Action<SerializationSettings> action)
    {
        action(serialization);
    }

    public static void IgnoreStackTrack()
    {
        ModifySerialization(_ => _.IgnoreMember("StackTrace"));
    }

    public static void AddExtraDateFormat(string format)
    {
        SerializationSettings.dateFormats.Add(format);
    }

    public static void AddExtraDatetimeFormat(string format)
    {
        SerializationSettings.datetimeFormats.Add(format);
    }

    public static void AddExtraDatetimeOffsetFormat(string format)
    {
        SerializationSettings.datetimeOffsetFormats.Add(format);
    }

    public static void UseStrictJson()
    {
        StrictJson = true;
    }

    public static bool StrictJson { get; private set; }

    internal static bool scrubProjectDir = true;

    public static void DontScrubProjectDirectory()
    {
        scrubProjectDir = false;
    }

    internal static bool scrubSolutionDir = true;

    public static void DontScrubSolutionDirectory()
    {
        scrubSolutionDir = false;
    }

    internal static bool sortPropertiesAlphabetically;

    public static void SortPropertiesAlphabetically()
    {
        sortPropertiesAlphabetically = true;
    }
}
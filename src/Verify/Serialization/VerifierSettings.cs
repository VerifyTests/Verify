using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        internal static SerializationSettings serialization = new SerializationSettings();

       internal static ConcurrentDictionary<Type, Func<object,VerifySettings, string>> typeToString = new ConcurrentDictionary<Type, Func<object,VerifySettings, string>>(
            new Dictionary<Type, Func<object,VerifySettings, string>>()
            {
                {typeof(string), (target, settings) => (string) target},
                {typeof(bool), (target, settings) => ((bool) target).ToString()},
                {typeof(DateTime), (target, settings) => ((DateTime) target).ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFz")},
                {typeof(DateTimeOffset), (target, settings) => ((DateTimeOffset) target).ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFz")},
                {typeof(short), (target, settings) => ((short) target).ToString()},
                {typeof(ushort), (target, settings) => ((ushort) target).ToString()},
                {typeof(int), (target, settings) => ((int) target).ToString()},
                {typeof(uint), (target, settings) => ((uint) target).ToString()},
                {typeof(long), (target, settings) => ((long) target).ToString()},
                {typeof(ulong), (target, settings) => ((ulong) target).ToString()},
                {typeof(decimal), (target, settings) => ((decimal) target).ToString(CultureInfo.InvariantCulture)},
                {typeof(float), (target, settings) => ((float) target).ToString(CultureInfo.InvariantCulture)},
                {typeof(Guid), (target, settings) => ((Guid) target).ToString()},
                {typeof(XmlNode), (target, settings) =>
                    {
                        var converted = (XmlNode) target;
                        var document = XDocument.Parse(converted.OuterXml);
                        settings.UseExtension("xml");
                        return document.ToString();
                    }
                },
                {typeof(XDocument), (target, settings) =>
                    {
                        var converted = (XDocument) target;
                        settings.UseExtension("xml");
                        return converted.ToString();
                    }
                },
            }
        );

        public static void TreatAsString<T>(Func<T, VerifySettings, string>? toString = null)
        {
            if (toString == null)
            {
                toString = (target, settings) =>
                {
                    if (target is null)
                    {
                        return "null";
                    }
                    return target.ToString();
                };
            }
            typeToString[typeof(T)] = (target, settings) => toString((T) target, settings);
        }

        public static void AddExtraSettings(Action<JsonSerializerSettings> action)
        {
            serialization.AddExtraSettings(action);
            serialization.RegenSettings();
        }

        public static void ModifySerialization(Action<SerializationSettings> action)
        {
            action(serialization);
            serialization.RegenSettings();
        }

        public static void AddExtraDatetimeFormat(string format)
        {
            SharedScrubber.datetimeFormats.Add(format);
        }

        public static void AddExtraDatetimeOffsetFormat(string format)
        {
            SharedScrubber.datetimeOffsetFormats.Add(format);
        }
    }
}
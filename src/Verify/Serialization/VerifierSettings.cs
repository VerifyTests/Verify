using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
// ReSharper disable UnusedParameter.Local

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        internal static SerializationSettings serialization = new();

        public static bool TryGetToString<T>(T target, out Func<object, IReadOnlyDictionary<string, object>, AsStringResult>? toString)
        {
            if (target is Type type)
            {
                toString = (_, _) => TypeNameConverter.GetName(type);
                return true;
            }

            if (target is FieldInfo field)
            {
                toString = (_, _) => TypeNameConverter.GetName(field);
                return true;
            }

            if (target is PropertyInfo property)
            {
                toString = (_, _) => TypeNameConverter.GetName(property);
                return true;
            }

            if (target is MethodInfo method)
            {
                toString = (_, _) => TypeNameConverter.GetName(method);
                return true;
            }

            if (target is ConstructorInfo constructor)
            {
                toString = (_, _) => TypeNameConverter.GetName(constructor);
                return true;
            }

            if (target is ParameterInfo parameter)
            {
                toString = (_, _) => TypeNameConverter.GetName(parameter);
                return true;
            }

            return typeToString.TryGetValue(target!.GetType(), out toString);
        }

        static Dictionary<Type, Func<object, IReadOnlyDictionary<string, object>, AsStringResult>> typeToString = new()
        {
            #region typeToStringMapping

            {typeof(string), (target, _) => (string) target},
            {typeof(bool), (target, _) => ((bool) target).ToString()},
            {typeof(short), (target, _) => ((short) target).ToString()},
            {typeof(ushort), (target, _) => ((ushort) target).ToString()},
            {typeof(int), (target, _) => ((int) target).ToString()},
            {typeof(uint), (target, _) => ((uint) target).ToString()},
            {typeof(long), (target, _) => ((long) target).ToString()},
            {typeof(ulong), (target, _) => ((ulong) target).ToString()},
            {typeof(decimal), (target, _) => ((decimal) target).ToString(CultureInfo.InvariantCulture)},
#if NET5_0
             //   {typeof(Half), (target, settings) => ((Half) target).ToString(CultureInfo.InvariantCulture)},
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
                    return dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFz");
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
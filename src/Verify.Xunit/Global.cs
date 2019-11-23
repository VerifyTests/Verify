using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace VerifyXunit
{
    public static class Global
    {
        internal static List<Func<string, string>> GlobalScrubbers = new List<Func<string, string>>();

        public static void AddScrubber(Func<string, string> scrubber)
        {
            Guard.AgainstNull(scrubber, nameof(scrubber));

            GlobalScrubbers.Insert(0, scrubber);
        }

        internal static SerializationSettings serializationSettings = new SerializationSettings();

        public static void IgnoreMember<T>(Expression<Func<T, object>> expression)
        {
            serializationSettings.IgnoreMember(expression);
        }

        public static void IgnoreMember(Type declaringType, string name)
        {
            serializationSettings.IgnoreMember(declaringType, name);
        }

        public static void IgnoreInstance<T>(Func<T, bool> shouldIgnore)
        {
            serializationSettings.IgnoreInstance(shouldIgnore);
        }

        public static void IgnoreInstance(Type type, Func<object, bool> shouldIgnore)
        {
            serializationSettings.IgnoreInstance(type, shouldIgnore);
        }

        public static void IgnoreMembersWithType<T>()
        {
            serializationSettings.IgnoreMembersWithType<T>();
        }

        public static void IgnoreMembersThatThrow<T>()
            where T : Exception
        {
            serializationSettings.IgnoreMembersThatThrow<T>();
        }

        public static void IgnoreMembersThatThrow(Func<Exception, bool> item)
        {
            serializationSettings.IgnoreMembersThatThrow(item);
        }

        public static void IgnoreMembersThatThrow<T>(Func<T, bool> item)
            where T : Exception
        {
            serializationSettings.IgnoreMembersThatThrow(item);
        }

        public static void DontIgnoreEmptyCollections()
        {
            serializationSettings.DontIgnoreEmptyCollections();
        }

        public static void DontIgnoreFalse()
        {
            serializationSettings.DontIgnoreFalse();
        }

        public static void DontScrubGuids()
        {
            serializationSettings.DontScrubGuids();
        }

        public static void DontScrubDateTimes()
        {
            serializationSettings.DontScrubDateTimes();
        }

        public static void ApplyExtraSettings(Action<JsonSerializerSettings> action)
        {
            serializationSettings.ExtraSettings = action;
        }
    }
}
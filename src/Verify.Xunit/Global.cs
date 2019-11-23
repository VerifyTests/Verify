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

        internal static SerializationSettings serialization = new SerializationSettings();

        public static void IgnoreMember<T>(Expression<Func<T, object>> expression)
        {
            serialization.IgnoreMember(expression);
        }

        public static void IgnoreMember(Type declaringType, string name)
        {
            serialization.IgnoreMember(declaringType, name);
        }

        public static void IgnoreInstance<T>(Func<T, bool> shouldIgnore)
        {
            serialization.IgnoreInstance(shouldIgnore);
        }

        public static void IgnoreInstance(Type type, Func<object, bool> shouldIgnore)
        {
            serialization.IgnoreInstance(type, shouldIgnore);
        }

        public static void IgnoreMembersWithType<T>()
        {
            serialization.IgnoreMembersWithType<T>();
        }

        public static void IgnoreMembersThatThrow<T>()
            where T : Exception
        {
            serialization.IgnoreMembersThatThrow<T>();
        }

        public static void IgnoreMembersThatThrow(Func<Exception, bool> item)
        {
            serialization.IgnoreMembersThatThrow(item);
        }

        public static void IgnoreMembersThatThrow<T>(Func<T, bool> item)
            where T : Exception
        {
            serialization.IgnoreMembersThatThrow(item);
        }

        public static void DontIgnoreEmptyCollections()
        {
            serialization.DontIgnoreEmptyCollections();
        }

        public static void DontIgnoreFalse()
        {
            serialization.DontIgnoreFalse();
        }

        public static void DontScrubGuids()
        {
            serialization.DontScrubGuids();
        }

        public static void DontScrubDateTimes()
        {
            serialization.DontScrubDateTimes();
        }

        public static void ApplyExtraSettings(Action<JsonSerializerSettings> action)
        {
            serialization.ApplyExtraSettings(action);
        }
    }
}
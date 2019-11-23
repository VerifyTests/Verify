using System;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        SerializationSettings serialization = Global.serialization.Clone();

        public void IgnoreMember<T>(Expression<Func<T, object>> expression)
        {
            serialization.IgnoreMember(expression);
        }

        public void IgnoreMember(Type declaringType, string name)
        {
            serialization.IgnoreMember(declaringType, name);
        }

        public void IgnoreInstance<T>(Func<T,bool> shouldIgnore)
        {
            serialization.IgnoreInstance(shouldIgnore);
        }

        public void IgnoreInstance(Type type, Func<object,bool> shouldIgnore)
        {
            serialization.IgnoreInstance(type, shouldIgnore);
        }

        public void IgnoreMembersWithType<T>()
        {
            serialization.IgnoreMembersWithType<T>();
        }

        public void IgnoreMembersThatThrow<T>()
            where T : Exception
        {
            serialization.IgnoreMembersThatThrow<T>();
        }

        public void IgnoreMembersThatThrow(Func<Exception, bool> item)
        {
            serialization.IgnoreMembersThatThrow(item);
        }

        public void IgnoreMembersThatThrow<T>(Func<T, bool> item)
            where T : Exception
        {
            serialization.IgnoreMembersThatThrow(item);
        }

        public void DontIgnoreEmptyCollections()
        {
            serialization.DontIgnoreEmptyCollections();
        }

        public void DontIgnoreFalse()
        {
            serialization.DontIgnoreFalse();
        }

        public void DontScrubGuids()
        {
            serialization.DontScrubGuids();
        }

        public void DontScrubDateTimes()
        {
            serialization.DontScrubDateTimes();
        }

        public void ApplyExtraSettings(Action<JsonSerializerSettings> action)
        {
            serialization.ApplyExtraSettings(action);
        }
    }
}
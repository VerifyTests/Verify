using System;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        SerializationSettings serializationSettings = Global.serializationSettings.Clone();

        public void IgnoreMember<T>(Expression<Func<T, object>> expression)
        {
            serializationSettings.IgnoreMember(expression);
        }

        public void IgnoreMember(Type declaringType, string name)
        {
            serializationSettings.IgnoreMember(declaringType, name);
        }

        public void IgnoreInstance<T>(Func<T,bool> shouldIgnore)
        {
            serializationSettings.IgnoreInstance(shouldIgnore);
        }

        public void IgnoreInstance(Type type, Func<object,bool> shouldIgnore)
        {
            serializationSettings.IgnoreInstance(type, shouldIgnore);
        }

        public void IgnoreMembersWithType<T>()
        {
            serializationSettings.IgnoreMembersWithType<T>();
        }

        public void IgnoreMembersThatThrow<T>()
            where T : Exception
        {
            serializationSettings.IgnoreMembersThatThrow<T>();
        }

        public void IgnoreMembersThatThrow(Func<Exception, bool> item)
        {
            serializationSettings.IgnoreMembersThatThrow(item);
        }

        public void IgnoreMembersThatThrow<T>(Func<T, bool> item)
            where T : Exception
        {
            serializationSettings.IgnoreMembersThatThrow(item);
        }

        public void DontIgnoreEmptyCollections()
        {
            serializationSettings.DontIgnoreEmptyCollections();
        }

        public void DontIgnoreFalse()
        {
            serializationSettings.DontIgnoreFalse();
        }

        public void DontScrubGuids()
        {
            serializationSettings.DontScrubGuids();
        }

        public void DontScrubDateTimes()
        {
            serializationSettings.DontScrubDateTimes();
        }

        public void ApplyExtraSettings(Action<JsonSerializerSettings> action)
        {
            serializationSettings.ExtraSettings = action;
        }
    }
}
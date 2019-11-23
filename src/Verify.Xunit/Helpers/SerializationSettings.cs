using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
// ReSharper disable UseObjectOrCollectionInitializer

namespace VerifyXunit
{
    class SerializationSettings
    {
        public SerializationSettings()
        {
            IgnoreMembersThatThrow<NotImplementedException>();
            IgnoreMembersThatThrow<NotSupportedException>();
            IgnoreMember<Exception>(x => x.HResult);
            IgnoreMember<Exception>(x => x.StackTrace);
        }

        Dictionary<Type, ConcurrentBag<string>> ignoreMembersByName = new Dictionary<Type, ConcurrentBag<string>>();
        Dictionary<Type, ConcurrentBag<Func<object, bool>>> ignoredInstances = new Dictionary<Type, ConcurrentBag<Func<object, bool>>>();


        public SerializationSettings Clone()
        {
            return new SerializationSettings
            {
                ignoreMembersByName = ignoreMembersByName.Clone(),
                ignoreEmptyCollections = ignoreEmptyCollections,
                ExtraSettings = ExtraSettings,
                ignoreFalse = ignoreFalse,
                ignoreMembersThatThrow = ignoreMembersThatThrow.Clone(),
                ignoreMembersWithType = ignoreMembersWithType.Clone(),
                ignoredInstances = ignoredInstances.Clone(),
                scrubDateTimes = scrubDateTimes,
                scrubGuids = scrubGuids
            };
        }

        public void IgnoreMember<T>(Expression<Func<T, object>> expression)
        {
            Guard.AgainstNull(expression, nameof(expression));
            if (expression.Body is UnaryExpression unary)
            {
                if (unary.Operand is MemberExpression unaryMember)
                {
                    var declaringType = unaryMember.Member.DeclaringType;
                    var memberName = unaryMember.Member.Name;
                    IgnoreMember(declaringType, memberName);
                    return;
                }
            }

            if (expression.Body is MemberExpression member)
            {
                var declaringType = member.Member.DeclaringType;
                var memberName = member.Member.Name;
                IgnoreMember(declaringType, memberName);
                return;
            }

            throw new ArgumentException("expression");
        }

        public void IgnoreMember(Type declaringType, string name)
        {
            Guard.AgainstNull(declaringType, nameof(declaringType));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            var list = ignoreMembersByName[declaringType] = new ConcurrentBag<string>();
            list.Add(name);
        }

        public void IgnoreInstance<T>(Func<T, bool> shouldIgnore)
        {
            Guard.AgainstNull(shouldIgnore, nameof(shouldIgnore));
            var type = typeof(T);
            IgnoreInstance(
                type,
                target =>
                {
                    var arg = (T) target;
                    return shouldIgnore(arg);
                });
        }

        public void IgnoreInstance(Type type, Func<object, bool> shouldIgnore)
        {
            Guard.AgainstNull(shouldIgnore, nameof(shouldIgnore));
            var list = ignoredInstances[type] = new ConcurrentBag<Func<object, bool>>();
            list.Add(shouldIgnore);
        }

        List<Type> ignoreMembersWithType = new List<Type>();

        public void IgnoreMembersWithType<T>()
        {
            ignoreMembersWithType.Add(typeof(T));
        }

        List<Func<Exception, bool>> ignoreMembersThatThrow = new List<Func<Exception, bool>>();

        public void IgnoreMembersThatThrow<T>()
            where T : Exception
        {
            ignoreMembersThatThrow.Add(x => x is T);
        }

        public void IgnoreMembersThatThrow(Func<Exception, bool> item)
        {
            IgnoreMembersThatThrow<Exception>(item);
        }

        public void IgnoreMembersThatThrow<T>(Func<T, bool> item)
            where T : Exception
        {
            Guard.AgainstNull(item, nameof(item));
            ignoreMembersThatThrow.Add(x =>
            {
                if (x is T exception)
                {
                    return item(exception);
                }

                return false;
            });
        }

        bool ignoreEmptyCollections;

        public void DontIgnoreEmptyCollections()
        {
            ignoreEmptyCollections = false;
        }

        bool ignoreFalse;

        public void DontIgnoreFalse()
        {
            ignoreFalse = false;
        }

        bool scrubGuids;

        public void DontScrubGuids()
        {
            scrubGuids = false;
        }

        bool scrubDateTimes;

        public void DontScrubDateTimes()
        {
            scrubDateTimes = false;
        }

        public JsonSerializerSettings BuildSettings()
        {
            #region defaultSerialization

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            #endregion

            settings.SerializationBinder = new ShortNameBinder();
            settings.ContractResolver = new CustomContractResolver(
                ignoreEmptyCollections,
                ignoreFalse,
                ignoreMembersByName,
                ignoreMembersWithType,
                ignoreMembersThatThrow,
                ignoredInstances);
            AddConverters(scrubGuids, scrubDateTimes, settings);
            ExtraSettings(settings);
            return settings;
        }

        public Action<JsonSerializerSettings> ExtraSettings = settings => { };

        void AddConverters(bool scrubGuids, bool scrubDateTimes, JsonSerializerSettings settings)
        {
            var converters = settings.Converters;
            converters.Add(new StringEnumConverter());
            converters.Add(new DelegateConverter());
            converters.Add(new TypeConverter());
            if (scrubGuids && scrubDateTimes)
            {
                var guidScrubbingConverter = new Scrubber<Guid>();
                converters.Add(guidScrubbingConverter);
                var dateTimeScrubber = new Scrubber<DateTime>();
                converters.Add(dateTimeScrubber);
                var dateTimeOffsetScrubber = new Scrubber<DateTimeOffset>();
                converters.Add(dateTimeOffsetScrubber);
                converters.Add(new StringScrubber(guidScrubbingConverter, dateTimeScrubber, dateTimeOffsetScrubber));
                return;
            }

            if (scrubGuids)
            {
                var guidScrubbingConverter = new Scrubber<Guid>();
                converters.Add(guidScrubbingConverter);
                converters.Add(new StringScrubber(guidScrubbingConverter, null, null));
            }

            if (scrubDateTimes)
            {
                var dateTimeScrubber = new Scrubber<DateTime>();
                converters.Add(dateTimeScrubber);
                var dateTimeOffsetScrubber = new Scrubber<DateTimeOffset>();
                converters.Add(dateTimeOffsetScrubber);
                converters.Add(new StringScrubber(null, dateTimeScrubber, dateTimeOffsetScrubber));
            }
        }
    }
}
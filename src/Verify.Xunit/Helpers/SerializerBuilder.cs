using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
// ReSharper disable UseObjectOrCollectionInitializer

namespace ObjectApproval
{
    public static class SerializerBuilder
    {
        static SerializerBuilder()
        {
            Reset();
        }

        public static void Reset()
        {
            ignoreMembersByName.Clear();
            ignoredInstances.Clear();
            ignoreMembersWithType.Clear();
            ignoreMembersThatThrow.Clear();

            IgnoreEmptyCollections = true;
            IgnoreFalse = true;
            ScrubGuids = true;
            ScrubDateTimes = true;

            IgnoreMembersThatThrow<NotImplementedException>();
            IgnoreMembersThatThrow<NotSupportedException>();
            IgnoreMember<Exception>(x => x.HResult);
            IgnoreMember<Exception>(x => x.StackTrace);

            ExtraSettings = settings => { };
        }

        static ConcurrentDictionary<Type, ConcurrentBag<string>> ignoreMembersByName = new ConcurrentDictionary<Type, ConcurrentBag<string>>();
        static ConcurrentDictionary<Type, ConcurrentBag<Func<object,bool>>> ignoredInstances = new ConcurrentDictionary<Type, ConcurrentBag<Func<object,bool>>>();

        public static void IgnoreMember<T>(Expression<Func<T, object>> expression)
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

        public static void IgnoreMember(Type declaringType, string name)
        {
            Guard.AgainstNull(declaringType, nameof(declaringType));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            var list = ignoreMembersByName.GetOrAdd(declaringType, _ => new ConcurrentBag<string>());
            list.Add(name);
        }

        public static void IgnoreInstance<T>(Func<T,bool> shouldIgnore)
        {
            Guard.AgainstNull(shouldIgnore, nameof(shouldIgnore));
            var type = typeof(T);
            IgnoreInstance(
                type,
                target =>
                {
                    var arg = (T)target;
                    return shouldIgnore(arg);
                });
        }

        public static void IgnoreInstance(Type type, Func<object,bool> shouldIgnore)
        {
            Guard.AgainstNull(shouldIgnore, nameof(shouldIgnore));
            var list = ignoredInstances.GetOrAdd(type, _ => new ConcurrentBag<Func<object,bool>>());
            list.Add(shouldIgnore);
        }

        static List<Type> ignoreMembersWithType = new List<Type>();

        public static void IgnoreMembersWithType<T>()
        {
            ignoreMembersWithType.Add(typeof(T));
        }

        static List<Func<Exception, bool>> ignoreMembersThatThrow = new List<Func<Exception, bool>>();

        public static void IgnoreMembersThatThrow<T>()
            where T : Exception
        {
            ignoreMembersThatThrow.Add(x => x is T);
        }

        public static void IgnoreMembersThatThrow(Func<Exception, bool> item)
        {
            IgnoreMembersThatThrow<Exception>(item);
        }

        public static void IgnoreMembersThatThrow<T>(Func<T, bool> item)
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

        public static bool IgnoreEmptyCollections { get; set; }
        public static bool IgnoreFalse { get; set; }
        public static bool ScrubGuids { get; set; }
        public static bool ScrubDateTimes { get; set; }

        public static JsonSerializerSettings BuildSettings(
            bool? ignoreEmptyCollections = null,
            bool? scrubGuids = null,
            bool? scrubDateTimes = null,
            bool? ignoreFalse = null)
        {
            var ignoreEmptyCollectionsVal = ignoreEmptyCollections.GetValueOrDefault(IgnoreEmptyCollections);
            var ignoreFalseVal = ignoreFalse.GetValueOrDefault(IgnoreFalse);
            var scrubGuidsVal = scrubGuids.GetValueOrDefault(ScrubGuids);
            var scrubDateTimesVal = scrubDateTimes.GetValueOrDefault(ScrubDateTimes);

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
                ignoreEmptyCollectionsVal,
                ignoreFalseVal,
                ignoreMembersByName,
                ignoreMembersWithType,
                ignoreMembersThatThrow,
                ignoredInstances);
            AddConverters(scrubGuidsVal, scrubDateTimesVal, settings);
            ExtraSettings(settings);
            return settings;
        }

        public static Action<JsonSerializerSettings> ExtraSettings = null!;

        static void AddConverters(bool scrubGuids, bool scrubDateTimes, JsonSerializerSettings settings)
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
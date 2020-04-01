using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable UseObjectOrCollectionInitializer

namespace Verify
{
    public class SerializationSettings
    {
        public SerializationSettings()
        {
            IgnoreMembersThatThrow<NotImplementedException>();
            IgnoreMembersThatThrow<NotSupportedException>();
            IgnoreMember<Exception>(x => x.HResult);
            IgnoreMember<Exception>(x => x.StackTrace);

            currentSettings = BuildSettings();
        }

        Dictionary<Type, List<string>> ignoredMembers = new Dictionary<Type, List<string>>();
        List<string> ignoredByNameMembers = new List<string>();
        Dictionary<Type, List<Func<object, bool>>> ignoredInstances = new Dictionary<Type, List<Func<object, bool>>>();

        public SerializationSettings Clone()
        {
            return new SerializationSettings
            {
                ignoredMembers = ignoredMembers.Clone(),
                ignoredByNameMembers = ignoredByNameMembers.Clone(),
                ignoreEmptyCollections = ignoreEmptyCollections,
                ExtraSettings = ExtraSettings.Clone(),
                ignoreFalse = ignoreFalse,
                ignoreMembersThatThrow = ignoreMembersThatThrow.Clone(),
                ignoreMembersWithType = ignoreMembersWithType.Clone(),
                ignoredInstances = ignoredInstances.Clone(),
                scrubDateTimes = scrubDateTimes,
                scrubGuids = scrubGuids,
                includeObsoletes = includeObsoletes,
            };
        }

        public void IgnoreMember<T>(Expression<Func<T, object>> expression)
        {
            Guard.AgainstNull(expression, nameof(expression));
            var member = expression.FindMember();
            IgnoreMember(member.DeclaringType, member.Name);
        }

        public void IgnoreMember(Type declaringType, string name)
        {
            Guard.AgainstNull(declaringType, nameof(declaringType));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            if (!ignoredMembers.TryGetValue(declaringType, out var list))
            {
                ignoredMembers[declaringType] = list = new List<string>();
            }

            list.Add(name);
        }

        public void IgnoreMember(string name)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            ignoredByNameMembers.Add(name);
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
            Guard.AgainstNull(type, nameof(type));
            Guard.AgainstNull(shouldIgnore, nameof(shouldIgnore));

            if (!ignoredInstances.TryGetValue(type, out var list))
            {
                ignoredInstances[type] = list = new List<Func<object, bool>>();
            }

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

        bool ignoreEmptyCollections = true;

        public void DontIgnoreEmptyCollections()
        {
            ignoreEmptyCollections = false;
        }

        bool ignoreFalse = true;

        public void DontIgnoreFalse()
        {
            ignoreFalse = false;
        }

        bool scrubGuids = true;

        public void DontScrubGuids()
        {
            scrubGuids = false;
        }

        bool scrubDateTimes = true;

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
                includeObsoletes,
                ignoredMembers,
                ignoredByNameMembers,
                ignoreMembersWithType,
                ignoreMembersThatThrow,
                ignoredInstances);
            AddConverters(scrubGuids, scrubDateTimes, settings);
            foreach (var extraSetting in ExtraSettings)
            {
                extraSetting(settings);
            }
            return settings;
        }

        public void AddExtraSettings(Action<JsonSerializerSettings> action)
        {
            Guard.AgainstNull(action, nameof(action));
            ExtraSettings.Add(action);
        }

        List<Action<JsonSerializerSettings>> ExtraSettings = new List<Action<JsonSerializerSettings>>();
        internal JsonSerializerSettings currentSettings;

        static void AddConverters(bool scrubGuids, bool scrubDateTimes, JsonSerializerSettings settings)
        {
            var converters = settings.Converters;
            var sharedScrubber = new SharedScrubber(scrubGuids, scrubDateTimes);
            converters.Add(new StringScrubbingConverter(sharedScrubber));
            converters.Add(new GuidConverter(sharedScrubber));
            converters.Add(new DateTimeConverter(sharedScrubber));
            converters.Add(new DateTimeOffsetConverter(sharedScrubber));
            converters.Add(new StringEnumConverter());
            converters.Add(new DelegateConverter());
            converters.Add(new TypeJsonConverter());
        }

        internal void RegenSettings()
        {
            currentSettings = BuildSettings();
        }

        bool includeObsoletes;

        public void IncludeObsoletes()
        {
            includeObsoletes = true;
        }
    }
}
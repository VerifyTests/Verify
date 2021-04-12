using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable UseObjectOrCollectionInitializer

namespace VerifyTests
{
    public partial class SerializationSettings
    {
        static FileInfoConverter fileInfoConverter = new();
        static UriConverter uriConverter = new();
        static DirectoryInfoConverter directoryInfoConverter = new();
        static StringEnumConverter stringEnumConverter = new();
        static DelegateConverter delegateConverter = new();
        static ExpressionConverter expressionConverter = new();
        static TypeJsonConverter typeJsonConverter = new();
        static MethodInfoConverter methodInfoConverter = new();
        static FieldInfoConverter fieldInfoConverter = new();
        static ConstructorInfoConverter constructorInfoConverter = new();
        static ParameterInfoConverter parameterInfoConverter = new();
        static VersionConverter versionConverter = new();
        static PropertyInfoConverter propertyInfoConverter = new();

        public SerializationSettings()
        {
            IgnoreMembersThatThrow<NotImplementedException>();
            IgnoreMembersThatThrow<NotSupportedException>();
            IgnoreMember<AggregateException>(x => x.InnerException);
            IgnoreMember<Exception>(x => x.Source);
            IgnoreMember<Exception>(x => x.HResult);
            MemberConverter<Exception, string>(x => x.StackTrace, (_, value) => Scrubbers.ScrubStackTrace(value));

            currentSettings = BuildSettings();
        }

        internal Dictionary<Type, Dictionary<string, ConvertMember>> membersConverters = new();

        public SerializationSettings Clone()
        {
            return new()
            {
                membersConverters = membersConverters
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value.ToDictionary(y => y.Key, y => y.Value)),
                ignoredMembers = ignoredMembers.ToDictionary(
                    x => x.Key,
                    x => x.Value.Clone()),
                ignoredByNameMembers = ignoredByNameMembers.Clone(),
                ignoreEmptyCollections = ignoreEmptyCollections,
                ExtraSettings = ExtraSettings.Clone(),
                ignoreFalse = ignoreFalse,
                ignoreMembersThatThrow = ignoreMembersThatThrow.Clone(),
                ignoreMembersWithType = ignoreMembersWithType.Clone(),
                ignoredInstances = ignoredInstances
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value.Clone()),
                scrubDateTimes = scrubDateTimes,
                scrubGuids = scrubGuids,
                includeObsoletes = includeObsoletes,
            };
        }

        public void MemberConverter<TTarget, TMember>(
            Expression<Func<TTarget, TMember?>> expression,
            ConvertMember<TTarget, TMember?> converter)
        {
            Guard.AgainstNull(expression, nameof(expression));
            Guard.AgainstNull(converter, nameof(converter));
            var member = expression.FindMember();
            MemberConverter(
                member.DeclaringType!,
                member.Name,
                (target, memberValue) => converter((TTarget) target!, (TMember) memberValue!));
        }

        public void MemberConverter(Type declaringType, string name, ConvertMember converter)
        {
            Guard.AgainstNull(declaringType, nameof(declaringType));
            Guard.AgainstNull(converter, nameof(converter));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            if (!membersConverters.TryGetValue(declaringType, out var list))
            {
                membersConverters[declaringType] = list = new();
            }

            list[name] = converter;
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

            JsonSerializerSettings settings = new()
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            #endregion

            settings.SerializationBinder = new ShortNameBinder();
            SharedScrubber scrubber = new(scrubGuids, scrubDateTimes, settings);
            settings.ContractResolver = new CustomContractResolver(
                ignoreEmptyCollections,
                ignoreFalse,
                includeObsoletes,
                ignoredMembers,
                ignoredByNameMembers,
                ignoreMembersWithType,
                ignoreMembersThatThrow,
                ignoredInstances,
                scrubber,
                membersConverters);
            var converters = settings.Converters;
            converters.Add(new StringConverter(scrubber));
            converters.Add(new GuidConverter(scrubber));
            converters.Add(new DateTimeConverter(scrubber));
            converters.Add(new DateTimeOffsetConverter(scrubber));
            converters.Add(fileInfoConverter);
            converters.Add(uriConverter);
            converters.Add(directoryInfoConverter);
            converters.Add(stringEnumConverter);
            converters.Add(expressionConverter);
            converters.Add(delegateConverter);
            converters.Add(versionConverter);
            converters.Add(typeJsonConverter);
            converters.Add(methodInfoConverter);
            converters.Add(fieldInfoConverter);
            converters.Add(constructorInfoConverter);
            converters.Add(propertyInfoConverter);
            converters.Add(parameterInfoConverter);
            converters.Add(new HttpHeadersConverter(ignoredByNameMembers));
            converters.Add(new DictionaryConverter(ignoredByNameMembers));
            converters.Add(new JArrayConverter());
            converters.Add(new JObjectConverter(ignoredByNameMembers));
            converters.Add(new NameValueCollectionConverter(ignoredByNameMembers));
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

        List<Action<JsonSerializerSettings>> ExtraSettings = new();
        internal JsonSerializerSettings currentSettings;

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
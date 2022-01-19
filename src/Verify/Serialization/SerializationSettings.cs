using System.Globalization;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable UseObjectOrCollectionInitializer

namespace VerifyTests;

public partial class SerializationSettings
{
    static JArrayConverter jArrayConverter = new();
    static FileInfoConverter fileInfoConverter = new();
#if NET6_0_OR_GREATER
    static TimeConverter timeConverter = new();
#endif
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
    static ClaimConverter claimConverter = new();
    static ClaimsPrincipalConverter claimsPrincipalConverter = new();
    static ClaimsIdentityConverter claimsIdentityConverter = new();
    static JObjectConverter jObjectConverter = new();
    static NameValueCollectionConverter nameValueCollectionConverter = new();

    JsonSerializerSettings serializersettings;

    public SerializationSettings()
    {
        IgnoreMembersThatThrow<NotImplementedException>();
        IgnoreMembersThatThrow<NotSupportedException>();
        IgnoreMember<AggregateException>(x => x.InnerException);
        IgnoreMember<Exception>(x => x.Source);
        IgnoreMember<Exception>(x => x.HResult);
        MemberConverter<Exception, string>(x => x.StackTrace, (_, value) => Scrubbers.ScrubStackTrace(value));

        serializersettings = BuildSettings();
    }

    public SerializationSettings(SerializationSettings settings)
    {
        membersConverters = settings.membersConverters
            .ToDictionary(
                x => x.Key,
                x => x.Value.ToDictionary(y => y.Key, y => y.Value));
        ignoredMembers = settings.ignoredMembers.ToDictionary(
            x => x.Key,
            x => x.Value.Clone());
        ignoredByNameMembers = settings.ignoredByNameMembers.Clone();
        ignoreEmptyCollections = settings.ignoreEmptyCollections;
        extraSettings = settings.extraSettings.Clone();
        dontIgnoreFalse = settings.dontIgnoreFalse;
        ignoreMembersThatThrow = settings.ignoreMembersThatThrow.Clone();
        ignoredTypes = settings.ignoredTypes.Clone();
        ignoredInstances = settings.ignoredInstances
            .ToDictionary(
                x => x.Key,
                x => x.Value.Clone());
        scrubDateTimes = settings.scrubDateTimes;
        scrubNumericIds = settings.scrubNumericIds;
        scrubGuids = settings.scrubGuids;
        includeObsoletes = settings.includeObsoletes;
        isNumericId = settings.isNumericId;

        serializersettings = BuildSettings();
    }

    internal Dictionary<Type, Dictionary<string, ConvertMember>> membersConverters = new();

    public void MemberConverter<TTarget, TMember>(
        Expression<Func<TTarget, TMember?>> expression,
        ConvertMember<TTarget, TMember?> converter)
    {
        var member = expression.FindMember();
        MemberConverter(
            member.DeclaringType!,
            member.Name,
            (target, memberValue) => converter((TTarget) target!, (TMember) memberValue!));
    }

    public void MemberConverter(Type declaringType, string name, ConvertMember converter)
    {
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

    bool scrubNumericIds = true;

    private IsNumericId isNumericId = member => member.Name.EndsWith("Id");

    public void TreatAsNumericId(IsNumericId isNumericId)
    {
        this.isNumericId = isNumericId;
    }

    public void DontScrubNumericIds()
    {
        scrubNumericIds = false;
    }

    JsonSerializerSettings BuildSettings()
    {
        #region defaultSerialization

        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Culture = CultureInfo.InvariantCulture
        };

        #endregion

        settings.SerializationBinder = ShortNameBinder.Instance;
        var scrubber = new SharedScrubber(scrubGuids, scrubDateTimes, settings);
        var propertyIgnorer = new PropertyIgnorer(
            ignoreEmptyCollections,
            includeObsoletes,
            ignoredMembers,
            ignoredByNameMembers,
            ignoredTypes,
            ignoredInstances);
        settings.ContractResolver = new CustomContractResolver(
            dontIgnoreFalse,
            scrubNumericIds,
            isNumericId,
            ignoreMembersThatThrow,
            scrubber,
            membersConverters,
            propertyIgnorer);
        var converters = settings.Converters;
        converters.Add(new StringConverter(scrubber));
        converters.Add(new StringBuilderConverter(scrubber));
        converters.Add(new TextWriterConverter(scrubber));
        converters.Add(new GuidConverter(scrubber));
        converters.Add(new DateTimeConverter(scrubber));
#if NET6_0_OR_GREATER
        converters.Add(new DateConverter(scrubber));
        converters.Add(timeConverter);
#endif
        converters.Add(new DateTimeOffsetConverter(scrubber));
        converters.Add(fileInfoConverter);
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
        converters.Add(claimConverter);
        converters.Add(claimsIdentityConverter);
        converters.Add(claimsPrincipalConverter);
        converters.Add(new DictionaryConverter(ignoredByNameMembers));
        converters.Add(jArrayConverter);
        converters.Add(jObjectConverter);
        converters.Add(nameValueCollectionConverter);
        foreach (var extraSetting in extraSettings)
        {
            extraSetting(settings);
        }

        return settings;
    }

    static void ValidateSettings(JsonSerializerSettings settings)
    {
        if (settings.DateFormatHandling != DateFormatHandling.IsoDateFormat)
        {
            throw new("Custom DateFormatHandling is not supported. Instead use VerifierSettings.TreatAsString<DateTime>(func) to define custom handling.");
        }

        if (settings.DateFormatString != "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK")
        {
            throw new("Custom DateFormatString is not supported. Instead use VerifierSettings.TreatAsString<DateTime>(func) to define custom handling.");
        }

        if (settings.DateTimeZoneHandling != DateTimeZoneHandling.RoundtripKind)
        {
            throw new("Custom RoundtripKind is not supported. Instead use VerifierSettings.TreatAsString<DateTime>(func) to define custom handling.");
        }
    }

    public void AddExtraSettings(Action<JsonSerializerSettings> action)
    {
        extraSettings.Add(action);
        action(serializersettings);
        serializer = null;
    }

    List<Action<JsonSerializerSettings>> extraSettings = new();
    JsonSerializer? serializer;

    internal JsonSerializer Serializer
    {
        get
        {
            var jsonSerializer = serializer;
            if (jsonSerializer == null)
            {
                ValidateSettings(serializersettings);
                return serializer = JsonSerializer.Create(serializersettings);
            }

            return jsonSerializer;
        }
    }

    bool includeObsoletes;

    public void IncludeObsoletes()
    {
        includeObsoletes = true;
    }
}
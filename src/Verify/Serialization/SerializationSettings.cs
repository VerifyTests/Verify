// ReSharper disable UseObjectOrCollectionInitializer

using Formatting = Argon.Formatting;

partial class SerializationSettings
{
    static JArrayConverter jArrayConverter = new();
    static JObjectConverter jObjectConverter = new();
    static KeyValuePairConverter keyValuePairConverter = new();
    static InfoBuilder.Converter infoConverter = new();
#if NET6_0_OR_GREATER
    static TimeConverter timeConverter = new();
    static DateConverter dateConverter = new();
#endif
    static StringEnumConverter stringEnumConverter = new();
    static DelegateConverter delegateConverter = new();
    static TargetInvocationExceptionConverter targetInvocationExceptionConverter = new();
    static ToAppendConverter toAppendConverter = new();
    static ExpressionConverter expressionConverter = new();
    static TypeJsonConverter typeJsonConverter = new();
    static MethodInfoConverter methodInfoConverter = new();
    static FieldInfoConverter fieldInfoConverter = new();
    static ConstructorInfoConverter constructorInfoConverter = new();
    static ParameterInfoConverter parameterInfoConverter = new();
    static PropertyInfoConverter propertyInfoConverter = new();
    static ClaimConverter claimConverter = new();
    static AggregateExceptionConverter aggregateExceptionConverter = new();
    static ClaimsPrincipalConverter claimsPrincipalConverter = new();
    static ClaimsIdentityConverter claimsIdentityConverter = new();
    static NameValueCollectionConverter nameValueCollectionConverter = new();
    static StringDictionaryConverter stringDictionaryConverter = new();
    static TaskConverter taskConverter = new();
    static ValueTaskConverter valueTaskConverter = new();

    JsonSerializerSettings jsonSettings;

    public SerializationSettings()
    {
        IgnoreMembersThatThrow<NotImplementedException>();
        IgnoreMembersThatThrow<NotSupportedException>();
        IgnoreMembers<Exception>("Source", "HResult");
        IgnoreMembersWithType<Stream>();

        jsonSettings = BuildSettings();
    }

    public SerializationSettings(SerializationSettings settings)
    {
        ignoredMembers = settings.ignoredMembers.ToDictionary(
            _ => _.Key,
            _ => _.Value.Clone());
        ignoredByNameMembers = settings.ignoredByNameMembers.Clone();
        ignoreEmptyCollections = settings.ignoreEmptyCollections;
        extraSettings = settings.extraSettings.Clone();
        ignoreMembersThatThrow = settings.ignoreMembersThatThrow.Clone();
        ignoredTypes = settings.ignoredTypes.Clone();
        ignoredInstances = settings.ignoredInstances
            .ToDictionary(
                _ => _.Key,
                _ => _.Value.Clone());
        scrubDateTimes = settings.scrubDateTimes;
        enumerableInterceptors = new(settings.enumerableInterceptors);
        scrubGuids = settings.scrubGuids;
        includeObsoletes = settings.includeObsoletes;

        jsonSettings = BuildSettings();
    }

    bool scrubGuids = true;

    public void DontScrubGuids() =>
        scrubGuids = false;

    bool scrubDateTimes = true;

    public void DontScrubDateTimes() =>
        scrubDateTimes = false;

    JsonSerializerSettings BuildSettings()
    {
        #region defaultSerialization

        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        #endregion

        settings.SerializationBinder = ShortNameBinder.Instance;

        settings.ContractResolver = new CustomContractResolver(this);
        var converters = settings.Converters;
        converters.Add(aggregateExceptionConverter);
        converters.Add(infoConverter);
#if NET6_0_OR_GREATER
        converters.Add(dateConverter);
        converters.Add(timeConverter);
#endif
        converters.Add(stringEnumConverter);
        converters.Add(expressionConverter);
        converters.Add(delegateConverter);
        converters.Add(targetInvocationExceptionConverter);
        converters.Add(toAppendConverter);
        converters.Add(typeJsonConverter);
        converters.Add(methodInfoConverter);
        converters.Add(fieldInfoConverter);
        converters.Add(constructorInfoConverter);
        converters.Add(propertyInfoConverter);
        converters.Add(parameterInfoConverter);
        converters.Add(claimConverter);
        converters.Add(claimsIdentityConverter);
        converters.Add(taskConverter);
        converters.Add(valueTaskConverter);
        converters.Add(claimsPrincipalConverter);
        converters.Add(jArrayConverter);
        converters.Add(jObjectConverter);
        converters.Add(nameValueCollectionConverter);
        converters.Add(stringDictionaryConverter);
        converters.Add(keyValuePairConverter);
        foreach (var extraSetting in extraSettings)
        {
            extraSetting(settings);
        }

        return settings;
    }

    public void AddExtraSettings(Action<JsonSerializerSettings> action)
    {
        extraSettings.Add(action);
        action(jsonSettings);
        serializer = null;
    }

    List<Action<JsonSerializerSettings>> extraSettings = [];
    JsonSerializer? serializer;

    internal JsonSerializer Serializer
    {
        get
        {
            var jsonSerializer = serializer;
            if (jsonSerializer is null)
            {
                return serializer = JsonSerializer.Create(jsonSettings);
            }

            return jsonSerializer;
        }
    }

    internal bool SortDictionaries = true;

    public void DontSortDictionaries() =>
        SortDictionaries = false;
}
// ReSharper disable UseObjectOrCollectionInitializer

using Formatting = Argon.Formatting;

partial class SerializationSettings
{
    static SerializationSettings()
    {
        var converters = DefaultContractResolver.Converters;
        converters.Remove(converters.OfType<Argon.KeyValuePairConverter>().Single());
        converters.AddRange(new JsonConverter[]
        {
            new JArrayConverter(),
            new JObjectConverter(),
            new KeyValuePairConverter(),
            new InfoBuilder.Converter(),
#if NET6_0_OR_GREATER
            new TimeConverter(),
            new DateConverter(),
#endif
            new StringEnumConverter(),
            new DelegateConverter(),
            new TargetInvocationExceptionConverter(),
            new ExpressionConverter(),
            new TypeJsonConverter(),
            new MethodInfoConverter(),
            new FieldInfoConverter(),
            new ConstructorInfoConverter(),
            new ParameterInfoConverter(),
            new PropertyInfoConverter(),
            new ClaimConverter(),
            new AggregateExceptionConverter(),
            new ClaimsPrincipalConverter(),
            new ClaimsIdentityConverter(),
            new NameValueCollectionConverter(),
            new StringDictionaryConverter(),
            new TaskConverter(),
            new ValueTaskConverter(),
        });
    }

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
            DefaultValueHandling = DefaultValueHandling.Ignore,
        };

        #endregion

        settings.SerializationBinder = ShortNameBinder.Instance;

        settings.ContractResolver = new CustomContractResolver(this);
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

    List<Action<JsonSerializerSettings>> extraSettings = new();
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
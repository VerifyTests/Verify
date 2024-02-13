class CustomContractResolver(SerializationSettings settings) :
    DefaultContractResolver
{
    protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
    {
        var contract = base.CreateDictionaryContract(objectType);
        contract.DictionaryKeyResolver = (name, original) => ResolveDictionaryKey(contract, name, original);
        if (settings.SortDictionaries)
        {
            contract.OrderByKey = true;
        }

        contract.InterceptSerializeItem = (key, value) =>
        {
            if (key is string stringKey &&
                settings.TryGetScrubOrIgnoreByName(stringKey, out var scrubOrIgnore))
            {
                return ToInterceptResult(scrubOrIgnore.Value);
            }

            if (value is not null &&
                settings.TryGetScrubOrIgnoreByInstance(value, out scrubOrIgnore))
            {
                return ToInterceptResult(scrubOrIgnore.Value);
            }

            return InterceptResult.Default;
        };

        return contract;
    }

    static InterceptResult ToInterceptResult(ScrubOrIgnore scrubOrIgnore)
    {
        if (scrubOrIgnore == ScrubOrIgnore.Ignore)
        {
            return InterceptResult.Ignore;
        }

        return InterceptResult.Replace("{Scrubbed}");
    }

    string ResolveDictionaryKey(JsonDictionaryContract contract, string name, object original)
    {
        var counter = Counter.Current;
        var keyType = contract.DictionaryKeyType;

#if NET6_0_OR_GREATER

        if (original is Date date)
        {
            if (settings.TryConvert(counter, date, out var result))
            {
                return result;
            }
        }

        if (original is Time time)
        {
            if (settings.TryConvert(counter, time, out var result))
            {
                return result;
            }
        }

#endif

        if (original is Guid guid)
        {
            if (settings.TryConvert(counter, guid, out var result))
            {
                return result;
            }
        }

        if (original is string stringValue)
        {
            if (settings.TryParseConvert(counter, stringValue, out var result))
            {
                return result;
            }
        }

        if (original is DateTime dateTime)
        {
            if (settings.TryConvert(counter, dateTime, out var result))
            {
                return result;
            }
        }

        if (original is DateTimeOffset dateTimeOffset)
        {
            if (settings.TryConvert(counter, dateTimeOffset, out var result))
            {
                return result;
            }
        }

        if (keyType == typeof(Type))
        {
            var type = Type.GetType(name);
            if (type is null)
            {
                throw new($"Could not load type `{name}`.");
            }

            return type.SimpleName();
        }

        return name;
    }

    static FieldInfo exceptionMessageField = typeof(Exception).GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic)!;

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var properties = base.CreateProperties(type, memberSerialization);
        if (type.IsException())
        {
            var stackTrace = properties.Single(_ => _.PropertyName == "StackTrace");
            properties.Remove(stackTrace);
            properties.Add(stackTrace);
            properties.Insert(0,
                new(typeof(string), typeof(Exception))
                {
                    PropertyName = "Type",
                    ValueProvider = new TypeNameProvider(type),
                    Ignored = false,
                    Readable = true,
                    Writable = false
                });
        }

        if (VerifierSettings.sortPropertiesAlphabetically)
        {
            properties = properties
                // Still honor explicit ordering
                .OrderBy(_ => _.Order ?? -1)
                .ThenBy(_ => _.PropertyName, StringComparer.Ordinal)
                .ToList();
        }

        return properties;
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization serialization)
    {
        var property = base.CreateProperty(member, serialization);

        var valueProvider = property.ValueProvider;
        var memberType = property.PropertyType;
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (memberType is null || valueProvider is null)
        {
            return property;
        }

        if (settings.TryGetScrubOrIgnore(member, out var scrubOrIgnore))
        {
            if (scrubOrIgnore == ScrubOrIgnore.Ignore)
            {
                property.Ignored = true;
            }
            else
            {
                property.PropertyType = typeof(string);
                property.ValueProvider = new ScrubbedProvider();
            }

            return property;
        }

        if (member.Name == "Message")
        {
            if (member.DeclaringType == typeof(ArgumentException))
            {
                valueProvider = new DynamicValueProvider(exceptionMessageField);
            }
        }

        if (memberType.IsException())
        {
            property.TypeNameHandling = TypeNameHandling.All;
        }

        if (property.PropertyType == typeof(bool))
        {
            property.DefaultValueHandling = DefaultValueHandling.Include;
        }

        property.ValueProvider = new CustomValueProvider(
            valueProvider,
            memberType,
            settings.ShouldIgnoreException,
            VerifierSettings.GetMemberConverter(member),
            settings);

        return property;
    }

    protected override JsonArrayContract CreateArrayContract(Type objectType)
    {
        var contract = base.CreateArrayContract(objectType);

        contract.InterceptSerializeItem = item =>
        {
            if (item is not null &&
                settings.TryGetScrubOrIgnoreByInstance(item, out var scrubOrIgnore))
            {
                return ToInterceptResult(scrubOrIgnore.Value);
            }

            return InterceptResult.Default;
        };

        if (contract.CollectionItemType != null &&
            settings.TryGetEnumerableInterceptors(contract.CollectionItemType, out var order))
        {
            contract.InterceptSerializeItems = _ => order(_);
        }

        return contract;
    }
}
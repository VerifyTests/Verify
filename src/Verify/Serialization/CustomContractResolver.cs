class CustomContractResolver :
    DefaultContractResolver
{
    SerializationSettings settings;

    public CustomContractResolver(SerializationSettings settings)
    {
        this.settings = settings;
        IgnoreSerializableInterface = true;
    }

    protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
    {
        var contract = base.CreateDictionaryContract(objectType);
        contract.DictionaryKeyResolver = value => ResolveDictionaryKey(contract, value);
        return contract;
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var properties = base.CreateProperties(type, memberSerialization);
        if (type.IsException())
        {
            var stackTrace = properties.Single(x => x.PropertyName == "StackTrace");
            properties.Remove(stackTrace);
            properties.Add(stackTrace);
            properties.Insert(0,
                new()
                {
                    PropertyName = "Type",
                    PropertyType = typeof(string),
                    ValueProvider = new TypeNameProvider(type),
                    Ignored = false,
                    Readable = true,
                    Writable = false,
                });
        }

        if (VerifierSettings.sortPropertiesAlphabetically)
        {
            properties = properties
                // Still honor explicit ordering
                .OrderBy(p => p.Order ?? -1)
                .ThenBy(p => p.PropertyName, StringComparer.Ordinal)
                .ToList();
        }

        return properties;
    }

    string ResolveDictionaryKey(JsonDictionaryContract contract, string value)
    {
        var counter = Counter.Current;
        var keyType = contract.DictionaryKeyType;
        if (keyType == typeof(Guid))
        {
            if (settings.TryParseConvertGuid(counter, value, out var result))
            {
                return result;
            }
        }

        if (keyType == typeof(DateTimeOffset))
        {
            if (settings.TryParseConvertDateTimeOffset(counter, value, out var result))
            {
                return result;
            }
        }

        if (keyType == typeof(DateTime))
        {
            if (settings.TryParseConvertDateTime(counter, value, out var result))
            {
                return result;
            }
        }

        if (keyType == typeof(Type))
        {
            var type = Type.GetType(value);
            if (type is null)
            {
                throw new($"Could not load type `{value}`.");
            }

            return type.SimpleName();
        }

        return value;
    }

    static FieldInfo exceptionMessageField = typeof(Exception).GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic)!;

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization serialization)
    {
        var property = base.CreateProperty(member, serialization);

        var valueProvider = property.ValueProvider;
        var memberType = property.PropertyType;
        if (memberType == null || valueProvider is null)
        {
            return property;
        }

        if (member.Name == "Message")
        {
            if (member.DeclaringType == typeof(ArgumentException))
            {
                valueProvider = new ExpressionValueProvider(exceptionMessageField);
            }
        }

        if (memberType.IsException())
        {
            property.TypeNameHandling = TypeNameHandling.All;
        }

        property.ConfigureIfBool(member, settings.dontIgnoreFalse);

        if (settings.ShouldIgnore(member))
        {
            property.Ignored = true;
            return property;
        }

        if (settings.TryGetShouldSerialize(memberType, valueProvider.GetValue, out var shouldSerialize))
        {
            property.ShouldSerialize = shouldSerialize;
        }

        var underlyingType = Nullable.GetUnderlyingType(memberType) ?? memberType;
        if (
            underlyingType == typeof(int) ||
            underlyingType == typeof(long) ||
            underlyingType == typeof(uint) ||
            underlyingType == typeof(ulong)
        )
        {
            if (settings.scrubNumericIds && settings.isNumericId(member))
            {
                property.Converter = new IdConverter();
                return property;
            }
        }

        property.ValueProvider = new CustomValueProvider(valueProvider, memberType, settings.ignoreMembersThatThrow, VerifierSettings.GetMemberConverter(member));

        return property;
    }

}
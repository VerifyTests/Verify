using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleInfoName;
using VerifyTests;

class CustomContractResolver :
    DefaultContractResolver
{
    bool dontIgnoreFalse;
    bool scrubNumericIds;
    IsNumericId isNumericId;
    IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow;
    SharedScrubber scrubber;
    IReadOnlyDictionary<Type, Dictionary<string, ConvertMember>> membersConverters;
    PropertyIgnorer propertyIgnorer;

    public CustomContractResolver(
        bool dontIgnoreFalse,
        bool scrubNumericIds,
        IsNumericId isNumericId,
        IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow,
        SharedScrubber scrubber,
        IReadOnlyDictionary<Type, Dictionary<string, ConvertMember>> membersConverters,
        PropertyIgnorer propertyIgnorer)
    {
        this.dontIgnoreFalse = dontIgnoreFalse;
        this.scrubNumericIds = scrubNumericIds;
        this.isNumericId = isNumericId;
        this.ignoreMembersThatThrow = ignoreMembersThatThrow;
        this.scrubber = scrubber;
        this.membersConverters = membersConverters;
        this.propertyIgnorer = propertyIgnorer;
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
        var keyType = contract.DictionaryKeyType;
        if (keyType == typeof(Guid))
        {
            if (scrubber.TryParseConvertGuid(value, out var result))
            {
                return result;
            }
        }

        if (keyType == typeof(DateTimeOffset))
        {
            if (scrubber.TryParseConvertDateTimeOffset(value, out var result))
            {
                return result;
            }
        }

        if (keyType == typeof(DateTime))
        {
            if (scrubber.TryParseConvertDateTime(value, out var result))
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
        var propertyType = property.PropertyType;
        if (propertyType == null || valueProvider is null)
        {
            return property;
        }

        if (member.Name == "Message")
        {
            if (member.DeclaringType == typeof(ArgumentException))
            {
                valueProvider = new ReflectionValueProvider(exceptionMessageField);
            }
        }

        if (propertyType.IsException())
        {
            property.TypeNameHandling = TypeNameHandling.All;
        }

        property.ConfigureIfBool(member, dontIgnoreFalse);

        if (propertyIgnorer.ShouldIgnore(member, propertyType, property))
        {
            property.Ignored = true;
            return property;
        }

        if (propertyIgnorer.TryGetShouldSerialize(propertyType, valueProvider.GetValue, out var shouldSerialize))
        {
            property.ShouldSerialize = shouldSerialize;
        }

        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        if (
            underlyingType == typeof(int) ||
            underlyingType == typeof(long) ||
            underlyingType == typeof(uint) ||
            underlyingType == typeof(ulong)
        )
        {
            if (scrubNumericIds && isNumericId(member))
            {
                property.Converter = new IdConverter();
                return property;
            }
        }

        ConvertMember? membersConverter = null;
        foreach (var pair in membersConverters)
        {
            if (pair.Key.IsAssignableFrom(property.DeclaringType))
            {
                pair.Value.TryGetValue(member.Name, out membersConverter);
                break;
            }
        }

        property.ValueProvider = new CustomValueProvider(valueProvider, propertyType, ignoreMembersThatThrow, membersConverter);

        return property;
    }
}
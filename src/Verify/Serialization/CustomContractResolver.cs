using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleInfoName;
using VerifyTests;

class CustomContractResolver :
    DefaultContractResolver
{
    bool ignoreEmptyCollections;
    bool ignoreFalse;
    bool includeObsoletes;
    bool scrubNumericIds;
    IsNumericId isNumericId;
    IReadOnlyDictionary<Type, List<string>> ignoredMembers;
    IReadOnlyList<string> ignoredByNameMembers;
    IReadOnlyList<Type> ignoredTypes;
    IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow;
    IReadOnlyDictionary<Type, List<Func<object, bool>>> ignoredInstances;
    SharedScrubber scrubber;
    Dictionary<Type, Dictionary<string, ConvertMember>> membersConverters;

    public CustomContractResolver(
        bool ignoreEmptyCollections,
        bool ignoreFalse,
        bool includeObsoletes,
        bool scrubNumericIds,
        IsNumericId isNumericId,
        IReadOnlyDictionary<Type, List<string>> ignoredMembers,
        IReadOnlyList<string> ignoredByNameMembers,
        IReadOnlyList<Type> ignoredTypes,
        IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow,
        IReadOnlyDictionary<Type, List<Func<object, bool>>> ignoredInstances,
        SharedScrubber scrubber,
        Dictionary<Type, Dictionary<string, ConvertMember>> membersConverters)
    {
        this.ignoreEmptyCollections = ignoreEmptyCollections;
        this.ignoreFalse = ignoreFalse;
        this.includeObsoletes = includeObsoletes;
        this.scrubNumericIds = scrubNumericIds;
        this.isNumericId = isNumericId;
        this.ignoredMembers = ignoredMembers;
        this.ignoredByNameMembers = ignoredByNameMembers;
        this.ignoredTypes = ignoredTypes;
        this.ignoreMembersThatThrow = ignoreMembersThatThrow;
        this.ignoredInstances = ignoredInstances;
        this.scrubber = scrubber;
        this.membersConverters = membersConverters;
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
                .OrderBy(p => p.Order ?? -1) // Still honor explicit ordering
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

        if (ignoreEmptyCollections)
        {
            property.SkipEmptyCollections(member);
        }

        property.ConfigureIfBool(member, ignoreFalse);

        if (ShouldIgnore(member, propertyType, property))
        {
            property.Ignored = true;
            return property;
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

        if (ignoredInstances.TryGetValue(propertyType, out var funcs))
        {
            property.ShouldSerialize = declaringInstance =>
            {
                var instance = valueProvider.GetValue(declaringInstance);

                if (instance is null)
                {
                    return false;
                }

                return funcs.All(func => !func(instance));
            };
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

    bool ShouldIgnore(MemberInfo member, Type propertyType, JsonProperty property)
    {
        if (!includeObsoletes)
        {
            if (member.GetCustomAttribute<ObsoleteAttribute>(true) is not null)
            {
                return true;
            }
        }

        if (ignoredTypes.Any(x => x.IsAssignableFrom(propertyType)))
        {
            return true;
        }

        var propertyName = property.UnderlyingName;
        if (propertyName is not null)
        {
            if (ignoredByNameMembers.Contains(propertyName))
            {
                return true;
            }

            foreach (var pair in ignoredMembers)
            {
                if (pair.Value.Contains(propertyName))
                {
                    if (pair.Key.IsAssignableFrom(property.DeclaringType))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
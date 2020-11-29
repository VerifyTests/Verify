using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VerifyTests;

class CustomContractResolver :
    DefaultContractResolver
{
    bool ignoreEmptyCollections;
    bool ignoreFalse;
    bool includeObsoletes;
    IReadOnlyDictionary<Type, List<string>> ignoredMembers;
    IReadOnlyList<string> ignoredByNameMembers;
    IReadOnlyList<Type> ignoredTypes;
    IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow;
    IReadOnlyDictionary<Type, List<Func<object, bool>>> ignoredInstances;
    SharedScrubber scrubber;
    Dictionary<Type, Dictionary<string, ConvertMember>> membersConverters;

    public CustomContractResolver(bool ignoreEmptyCollections,
        bool ignoreFalse,
        bool includeObsoletes,
        IReadOnlyDictionary<Type, List<string>> ignoredMembers,
        IReadOnlyList<string> ignoredByNameMembers,
        IReadOnlyList<Type> ignoredTypes,
        IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow,
        IReadOnlyDictionary<Type, List<Func<object, bool>>> ignoredInstances,
        SharedScrubber scrubber,
        Dictionary<Type, Dictionary<string, ConvertMember>> membersConverters)
    {
        Guard.AgainstNull(ignoredMembers, nameof(ignoredMembers));
        Guard.AgainstNull(ignoredTypes, nameof(ignoredTypes));
        Guard.AgainstNull(ignoreMembersThatThrow, nameof(ignoreMembersThatThrow));
        this.ignoreEmptyCollections = ignoreEmptyCollections;
        this.ignoreFalse = ignoreFalse;
        this.includeObsoletes = includeObsoletes;
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
                new JsonProperty
                {
                    PropertyName = "Type",
                    PropertyType = typeof(string),
                    ValueProvider = new TypeNameProvider(type),
                    Ignored = false,
                    Readable = true,
                    Writable = false,
                });
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
            if (type == null)
            {
                throw new($"Could not load type `{value}`.");
            }

            return TypeNameConverter.GetName(type);
        }

        return value;
    }

    static FieldInfo exceptionMessageField = typeof(Exception).GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic)!;

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization serialization)
    {
        var property = base.CreateProperty(member, serialization);

        var valueProvider = property.ValueProvider;
        var propertyType = property.PropertyType;
        if (propertyType == null || valueProvider == null)
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

        if (!includeObsoletes)
        {
            if (member.GetCustomAttribute<ObsoleteAttribute>(true) != null)
            {
                property.Ignored = true;
                return property;
            }
        }

        if (ignoredTypes.Any(x => x.IsAssignableFrom(propertyType)))
        {
            property.Ignored = true;
            return property;
        }

        var propertyName = property.PropertyName!;
        if (ignoredByNameMembers.Contains(propertyName))
        {
            property.Ignored = true;
            return property;
        }

        foreach (var pair in ignoredMembers)
        {
            if (pair.Value.Contains(propertyName))
            {
                if (pair.Key.IsAssignableFrom(property.DeclaringType))
                {
                    property.Ignored = true;
                    return property;
                }
            }
        }

        if (ignoredInstances.TryGetValue(propertyType, out var funcs))
        {
            property.ShouldSerialize = declaringInstance =>
            {
                var instance = valueProvider.GetValue(declaringInstance);

                if (instance == null)
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
}
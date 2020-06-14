using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VerifyTesting;

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

    public CustomContractResolver(
        bool ignoreEmptyCollections,
        bool ignoreFalse,
        bool includeObsoletes,
        IReadOnlyDictionary<Type, List<string>> ignoredMembers,
        IReadOnlyList<string> ignoredByNameMembers,
        IReadOnlyList<Type> ignoredTypes,
        IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow,
        IReadOnlyDictionary<Type, List<Func<object, bool>>> ignoredInstances,
        SharedScrubber scrubber)
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
        IgnoreSerializableInterface = true;
    }

    protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
    {
        var contract = base.CreateDictionaryContract(objectType);
        contract.DictionaryKeyResolver = value =>
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
                return TypeNameConverter.GetName(Type.GetType(value));
            }

            return value;
        };
        return contract;
    }


    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (property.PropertyType == null || property.ValueProvider == null)
        {
            return property;
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

        if (ignoredTypes.Any(x => x.IsAssignableFrom(property.PropertyType)))
        {
            property.Ignored = true;
            return property;
        }

        if (ignoredByNameMembers.Contains(property.PropertyName!))
        {
            property.Ignored = true;
            return property;
        }

        foreach (var keyValuePair in ignoredMembers)
        {
            if (keyValuePair.Value.Contains(property.PropertyName!))
            {
                if (keyValuePair.Key.IsAssignableFrom(property.DeclaringType))
                {
                    property.Ignored = true;
                    return property;
                }
            }
        }

        if (ignoredInstances.TryGetValue(property.PropertyType, out var funcs))
        {
            property.ShouldSerialize = declaringInstance =>
            {
                var instance = property.ValueProvider.GetValue(declaringInstance);

                if (instance == null)
                {
                    return false;
                }

                foreach (var func in funcs)
                {
                    if (func(instance))
                    {
                        return false;
                    }
                }

                return true;
            };
        }

        property.ValueProvider = new CustomValueProvider(property.ValueProvider, property.PropertyType, ignoreMembersThatThrow);

        return property;
    }
}
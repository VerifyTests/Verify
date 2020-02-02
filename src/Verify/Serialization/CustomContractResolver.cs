using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Verify;

class CustomContractResolver :
    DefaultContractResolver
{
    bool ignoreEmptyCollections;
    bool ignoreFalse;
    bool includeObsoletes;
    IReadOnlyDictionary<Type, List<string>> ignored;
    IReadOnlyList<Type> ignoredTypes;
    IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow;
    IReadOnlyDictionary<Type, List<Func<object, bool>>> ignoredInstances;

    public CustomContractResolver(
        bool ignoreEmptyCollections,
        bool ignoreFalse,
        bool includeObsoletes,
        IReadOnlyDictionary<Type, List<string>> ignored,
        IReadOnlyList<Type> ignoredTypes,
        IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow,
        IReadOnlyDictionary<Type, List<Func<object, bool>>> ignoredInstances)
    {
        Guard.AgainstNull(ignored, nameof(ignored));
        Guard.AgainstNull(ignoredTypes, nameof(ignoredTypes));
        Guard.AgainstNull(ignoreMembersThatThrow, nameof(ignoreMembersThatThrow));
        this.ignoreEmptyCollections = ignoreEmptyCollections;
        this.ignoreFalse = ignoreFalse;
        this.includeObsoletes = includeObsoletes;
        this.ignored = ignored;
        this.ignoredTypes = ignoredTypes;
        this.ignoreMembersThatThrow = ignoreMembersThatThrow;
        this.ignoredInstances = ignoredInstances;
        IgnoreSerializableInterface = true;
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

        foreach (var keyValuePair in ignored)
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
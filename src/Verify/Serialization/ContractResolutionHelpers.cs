﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VerifyTests;

public static class ContractResolutionHelpers
{
    public static void ConfigureIfBool(this JsonProperty property, MemberInfo member, bool ignoreFalse)
    {
        if (ignoreFalse)
        {
            if (property.PropertyType == typeof(bool))
            {
                property.DefaultValueHandling = DefaultValueHandling.Ignore;
                return;
            }

            if (property.PropertyType == typeof(bool?))
            {
                property.ShouldSerialize = instance =>
                {
                    var value = member.GetValue<bool?>(instance);
                    return value.GetValueOrDefault(false);
                };
            }
        }
        else
        {
            if (property.PropertyType == typeof(bool))
            {
                property.DefaultValueHandling = DefaultValueHandling.Include;
                return;
            }

            if (property.PropertyType == typeof(bool?))
            {
                property.DefaultValueHandling = DefaultValueHandling.Include;
                property.ShouldSerialize = instance =>
                {
                    var value = member.GetValue<bool?>(instance);
                    return value != null;
                };
            }
        }
    }

    public static void SkipEmptyCollections(this JsonProperty property, MemberInfo member)
    {
        var type = property.PropertyType;
        if (type is null)
        {
            return;
        }

        if (type == typeof(string))
        {
            return;
        }

        if (type.IsCollection() || type.IsDictionary())
        {
            property.ShouldSerialize = instance =>
            {
                // since inside IsCollection, it is safe to use IEnumerable
                var collection = member.GetValue<IEnumerable>(instance);

                return HasMembers(collection);
            };
        }
    }

    static bool HasMembers(IEnumerable? collection)
    {
        if (collection is null)
        {
            // if the list is null, we defer the decision to NullValueHandling
            return true;
        }

        // check to see if there is at least one item in the Enumerable
        return collection.GetEnumerator().MoveNext();
    }
}
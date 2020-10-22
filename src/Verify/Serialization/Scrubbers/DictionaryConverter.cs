using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VerifyTests;

class DictionaryConverter :
    WriteOnlyJsonConverter
{
    List<string> ignoredByNameMembers;

    public DictionaryConverter(List<string> ignoredByNameMembers)
    {
        this.ignoredByNameMembers = ignoredByNameMembers;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            return;
        }

        var valueType = value.GetType().GetGenericArguments().Last();
        var genericType = typeof(DictionaryWrapper<>).MakeGenericType(valueType);
        var instance = Activator.CreateInstance(genericType, ignoredByNameMembers, value);
        serializer.Serialize(writer, instance);
    }


    public override bool CanConvert(Type objectType)
    {
        if (objectType.IsGenericType)
        {
            var genericDefinition = objectType.GetGenericTypeDefinition();
            if (genericDefinition == typeof(Dictionary<,>) ||
                genericDefinition == typeof(SortedDictionary<,>) ||
                genericDefinition == typeof(ConcurrentDictionary<,>) ||
                genericDefinition == typeof(IReadOnlyDictionary<,>))
            {
                if (objectType.GetGenericArguments().First() == typeof(string))
                {
                    return true;
                }
            }
        }
        return false;
    }
}

class DictionaryWrapper<TValue> : Dictionary<string, TValue>
{
    public DictionaryWrapper(List<string> ignored, IDictionary<string, TValue> inner) :
        base(inner.Where(x => !ignored.Contains(x.Key))
            .ToDictionary(x => x.Key, x => x.Value))
    {
    }
}
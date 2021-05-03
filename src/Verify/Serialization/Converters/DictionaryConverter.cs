using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public override void WriteJson(
        JsonWriter writer,
        object? value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        if (value is null)
        {
            return;
        }

        var type = value.GetType();
        var valueType = type.GetGenericArguments().Last();
        var genericType = typeof(DictionaryWrapper<,>).MakeGenericType(valueType, type);
        var instance = Activator.CreateInstance(genericType, ignoredByNameMembers, value);
        serializer.Serialize(writer, instance);
    }

    public override bool CanConvert(Type objectType)
    {
        if (objectType.IsGenericType)
        {
            var definition = objectType.GetGenericTypeDefinition();
            if (definition == typeof(Dictionary<,>) ||
                definition == typeof(SortedDictionary<,>) ||
                definition == typeof(ConcurrentDictionary<,>) ||
                definition == typeof(ReadOnlyDictionary<,>))
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
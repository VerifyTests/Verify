using System.Collections.Immutable;
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

    public override bool CanConvert(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        if (!typeof(IDictionary).IsAssignableFrom(type))
        {
            return false;
        }

        var definition = type.GetGenericTypeDefinition();
        return definition == typeof(Dictionary<,>) ||
               definition == typeof(ImmutableDictionary<,>) ||
               definition == typeof(SortedDictionary<,>) ||
               definition == typeof(ImmutableSortedDictionary<,>) ||
               definition == typeof(ConcurrentDictionary<,>) ||
               definition == typeof(ReadOnlyDictionary<,>);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        var type = value.GetType();

        var genericArguments = type.GetGenericArguments();
        var valueType = genericArguments.Last();
        var keyType = genericArguments.First();
        var definition = type.GetGenericTypeDefinition();
        if (definition == typeof(SortedDictionary<,>) ||
            definition == typeof(ImmutableSortedDictionary<,>) )
        {
            if (keyType == typeof(string))
            {
                var genericType = typeof(StringDictionaryWrapper<,>).MakeGenericType(valueType, type);
                value = Activator.CreateInstance(genericType, ignoredByNameMembers, value)!;
            }
            else
            {
                var genericType = typeof(DictionaryWrapper<,,>).MakeGenericType(keyType, valueType, type);
                value = Activator.CreateInstance(genericType, value)!;
            }
        }
        else
        {
            if (keyType == typeof(string))
            {
                var genericType = typeof(OrderedStringDictionaryWrapper<,>).MakeGenericType(valueType, type);
                value = Activator.CreateInstance(genericType, ignoredByNameMembers, value)!;
            }
            else
            {
                var genericType = typeof(OrderedDictionaryWrapper<,,>).MakeGenericType(keyType, valueType, type);
                value = Activator.CreateInstance(genericType, value)!;
            }
        }

        serializer.Serialize(writer, value);
    }
}
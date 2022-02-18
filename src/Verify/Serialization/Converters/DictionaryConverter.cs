using System.Collections.ObjectModel;

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
               definition.Name == "ImmutableDictionary`2" ||
               definition == typeof(SortedDictionary<,>) ||
               definition.Name == "ImmutableSortedDictionary`2" ||
               definition == typeof(ConcurrentDictionary<,>) ||
               definition == typeof(ReadOnlyDictionary<,>);
    }

    public override void Write(VerifyJsonWriter writer, object value)
    {
        var type = value.GetType();

        var genericArguments = type.GetGenericArguments();
        var valueType = genericArguments.Last();
        var keyType = genericArguments.First();
        var definition = type.GetGenericTypeDefinition();
        if (definition == typeof(SortedDictionary<,>) ||
            definition.Name == "ImmutableSortedDictionary`2")
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

        writer.Serialize(value);
    }
}
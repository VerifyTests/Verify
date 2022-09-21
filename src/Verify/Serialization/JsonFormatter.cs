static class JsonFormatter
{
    public static StringBuilder AsJson(object? input, List<ToAppend> appends, VerifySettings settings, Counter counter)
    {
        if (appends.Any())
        {
            var dictionary = new MultiValueDictionary();
            dictionary.Add("target", input ?? "null");

            input = dictionary;
            foreach (var append in appends)
            {
                dictionary.Add(append.Name, append.Data);
            }
        }

        var builder = new StringBuilder();
        using var writer = new VerifyJsonWriter(builder, settings, counter);
        settings.Serializer.Serialize(writer, input);
        builder.FixNewlines();
        return builder;
    }
}

class MultiValueDictionary
{
    public class Converter : WriteOnlyJsonConverter<MultiValueDictionary>
    {
        public override void Write(VerifyJsonWriter writer, MultiValueDictionary value)
        {
            writer.WriteStartArray();
            foreach (var item in value.inner)
            {
                if (item is KeyValuePair<string, List<object>> pair)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(pair.Key);
                    if (pair.Value.Count == 1)
                    {
                        writer.Serialize(pair.Value.Single());
                    }
                    else
                    {
                        writer.Serialize(pair.Value);
                    }
                    writer.WriteEndObject();
                }
                else
                {
                    writer.Serialize(item);
                }
            }

            writer.WriteEndArray();
        }
    }

    List<object> inner = new();
    Dictionary<string, List<object>> innerDictionary = new();

    public void Add(string name, object value)
    {
        if (innerDictionary.TryGetValue(name, out var list))
        {
            list.Add(value);
            return;
        }

        var objects = new List<object>
        {
            value
        };
        innerDictionary.Add(name, objects);
        inner.Add(new KeyValuePair<string, List<object>>(name, objects));
    }
}
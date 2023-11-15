class InfoBuilder
{
    object? root;
    List<Item> inner = [];

    public InfoBuilder(object? root, List<ToAppend> appends)
    {
        this.root = root;
        foreach (var append in appends)
        {
            Add(append.Name, append.Data);
        }
    }

    void Add(string name, object value)
    {
        var item = inner.SingleOrDefault(_ => _.Key == name);
        if (item == null)
        {
            inner.Add(new(name, value));
        }
        else
        {
            item.Values.Add(value);
        }
    }

    class Item(string key, object value)
    {
        public string Key { get; } = key;

        public List<object> Values { get; } = [value];
    }

    public class Converter :
        WriteOnlyJsonConverter<InfoBuilder>
    {
        public override void Write(VerifyJsonWriter writer, InfoBuilder value)
        {
            var root = value.root;
            if (value.inner.Count == 0)
            {
                if (root == null ||
                    root == InnerVerifier.IgnoreTarget)
                {
                    writer.Serialize("null");
                }
                else
                {
                    writer.Serialize(root);
                }

                return;
            }

            writer.WriteStartObject();

            if (root != InnerVerifier.IgnoreTarget)
            {
                writer.WritePropertyName("target");
                if (root == null)
                {
                    writer.WriteValue("null");
                }
                else
                {
                    writer.Serialize(root);
                }
            }

            foreach (var item in value.inner)
            {
                writer.WritePropertyName(item.Key);
                if (writer.serialization.ShouldScrubByName(item.Key))
                {
                    writer.WriteValue("Scrubbed");
                    continue;
                }

                var values = item.Values;
                if (values.Count == 1)
                {
                    writer.Serialize(values[0]);
                }
                else
                {
                    writer.Serialize(values);
                }
            }

            writer.WriteEndObject();
        }
    }
}
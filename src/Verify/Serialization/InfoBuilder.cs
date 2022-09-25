class InfoBuilder
{
    object? root;
    List<Item> inner = new();

    public InfoBuilder(object? root) =>
        this.root = root;

    public void Add(string name, object value)
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

    public class Item
    {
        public string Key { get; }

        public Item(string key, object value)
        {
            Key = key;
            Values = new()
            {
                value
            };
        }

        public List<object> Values { get; }
    }

    public class Converter :
        WriteOnlyJsonConverter<InfoBuilder>
    {
        public override void Write(VerifyJsonWriter writer, InfoBuilder value)
        {
            if (value.inner.Count == 0)
            {
                if (value.root == null)
                {
                    writer.Serialize("null");
                }
                else
                {
                    writer.Serialize(value.root);
                }
                return;
            }

            writer.WriteStartObject();
                writer.WritePropertyName("target");
            if (value.root == null)
            {
                writer.WriteValue("null");
            }
            else
            {
                writer.Serialize(value.root);
            }

            foreach (var item in value.inner)
            {
                writer.WritePropertyName(item.Key);
                if (item.Values.Count == 1)
                {
                    writer.Serialize(item.Values.Single());
                }
                else
                {
                    writer.Serialize(item.Values);
                }
            }

            writer.WriteEndObject();
        }
    }
}
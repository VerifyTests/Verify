class ArrayConverter : WriteOnlyJsonConverter
{
    public override bool CanConvert(Type objectType) =>
        true;

    public override void Write(VerifyJsonWriter writer, object value)
    {
        var genericType = typeof(ArrayWrapper<>).MakeGenericType(value.GetType());
        var wrapper = Activator.CreateInstance(genericType, value)!;
        writer.Serialize(wrapper);
    }

    static IEnumerable<object?> NewMethod(VerifyJsonWriter writer, object value)
    {
        foreach (var item in (IEnumerable) value)
        {
            if (item is null)
            {
                yield return item;
            }
            else if (writer.settings.ShouldSerialize(item))
            {
                yield return item;
            }
        }
    }
}
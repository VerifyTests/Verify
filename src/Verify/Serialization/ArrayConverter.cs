class ArrayConverter : WriteOnlyJsonConverter
{
    public override bool CanConvert(Type objectType) =>
        true;

    static Type arrayWrapperType = typeof(ArrayWrapper<>);

    public override void Write(VerifyJsonWriter writer, object value)
    {
        var genericType = arrayWrapperType.MakeGenericType(value.GetType());
        var wrapper = Activator.CreateInstance(genericType, NewMethod(writer, value))!;
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
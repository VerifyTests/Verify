class ScrubbedProvider : IValueProvider
{
    public void SetValue(object target, object? value) =>
        throw new NotImplementedException();

    public object GetValue(object target) =>
        "{Scrubbed}";
}
class ScrubbedConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) =>
        writer.WriteRaw("{Scrubbed}");

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) =>
        throw new NotImplementedException();

    public override bool CanConvert(Type objectType) =>
        throw new NotImplementedException();
}
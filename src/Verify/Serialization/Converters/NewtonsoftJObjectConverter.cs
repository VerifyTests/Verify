class NewtonsoftJObjectConverter :
    WriteOnlyJsonConverter
{
    public override void Write(VerifyJsonWriter writer, object value)
    {
        var method = value.GetType().GetMethod("ToObject", new Type[] {typeof(Type)})!;
        var dictionary = method.Invoke(value,new object[]{typeof(Dictionary<string, object>)})!;
        writer.Serialize(dictionary);
    }

    public sealed override bool CanConvert(Type type) =>
        type.FullName == "Newtonsoft.Json.Linq.JObject";
}
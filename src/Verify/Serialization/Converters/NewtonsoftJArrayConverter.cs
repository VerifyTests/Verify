class NewtonsoftJArrayConverter :
    WriteOnlyJsonConverter
{
    public override void Write(VerifyJsonWriter writer, object value)
    {
        var method = value.GetType().GetMethod("ToObject", new Type[] {typeof(Type)})!;
        var list = method.Invoke(value,new object[]{typeof(List<object>)})!;
        writer.Serialize(list);
    }

    public sealed override bool CanConvert(Type type) =>
        type.FullName == "Newtonsoft.Json.Linq.JArray";
}
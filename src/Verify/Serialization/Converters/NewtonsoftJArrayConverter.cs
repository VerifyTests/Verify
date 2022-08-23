class NewtonsoftJArrayConverter :
    WriteOnlyJsonConverter
{
    static MethodInfo? method;

    static object[] parameters =
    {
        typeof(List<object>)
    };

    public override void Write(VerifyJsonWriter writer, object value)
    {
        if (method == null)
        {
            var type = Type.GetType("Newtonsoft.Json.Linq.JArray, Newtonsoft.Json")!;

            method = type.GetMethod("ToObject", new[]
            {
                typeof(Type)
            })!;
        }

        var converted = method.Invoke(value, parameters)!;
        writer.Serialize(converted);
    }

    public sealed override bool CanConvert(Type type) =>
        type.FullName == "Newtonsoft.Json.Linq.JArray";
}
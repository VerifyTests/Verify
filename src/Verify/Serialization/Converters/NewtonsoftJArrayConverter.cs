class NewtonsoftJArrayConverter :
    WriteOnlyJsonConverter
{
    static MethodInfo? method;

    static object[] parameters =
    {
        typeof(List<object>)
    };

    static Lazy<Func<object, object>> lazy;

    static NewtonsoftJArrayConverter() =>
        lazy = new(() => value =>
        {
            if (method == null)
            {
                var type = Type.GetType("Newtonsoft.Json.Linq.JArray, Newtonsoft.Json")!;

                method = type.GetMethod("ToObject", new[]
                {
                    typeof(Type)
                })!;
            }

            return method.Invoke(value, parameters)!;
        });

    public override void Write(VerifyJsonWriter writer, object value) =>
        writer.Serialize(lazy.Value(value));

    public sealed override bool CanConvert(Type type) =>
        type.FullName == "Newtonsoft.Json.Linq.JArray";
}
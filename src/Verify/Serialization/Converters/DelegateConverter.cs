using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class DelegateConverter :
    WriteOnlyJsonConverter<Delegate>
{
    public override void WriteJson(
        JsonWriter writer,
        Delegate @delegate,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Type");
        writer.WriteValue(@delegate.GetType().SimpleName());
        var declaringType = @delegate.Method.DeclaringType;
        if (declaringType is not null)
        {
            writer.WritePropertyName("Target");
            writer.WriteValue(declaringType.SimpleName());
        }

        writer.WritePropertyName("Method");
        var s = @delegate.Method.ToString()!;
        writer.WriteValue(CleanMethodName(s));
        writer.WriteEndObject();
    }

    static string CleanMethodName(string name)
    {
        var split = name.Split('<', '>', '(');
        if (split.Length > 2)
        {
            var list = split.ToList();
            list[2] = "(";
            return string.Concat(list);
        }

        return name;
    }
}
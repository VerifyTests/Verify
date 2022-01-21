using SimpleInfoName;
using VerifyTests;

class DelegateConverter :
    WriteOnlyJsonConverter<Delegate>
{
    public override void Write(VerifyJsonWriter writer, Delegate @delegate)
    {
        writer.WriteStartObject();
        //TODO:
        writer.WriteProperty(@delegate, @delegate.GetType().SimpleName(), "Type");
        var declaringType = @delegate.Method.DeclaringType;
        if (declaringType is not null)
        {
            writer.WriteProperty(@delegate, declaringType.SimpleName(), "Target");
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
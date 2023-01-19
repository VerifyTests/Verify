// ReSharper disable RedundantSuppressNullableWarningExpression

class DelegateConverter :
    WriteOnlyJsonConverter<Delegate>
{
    public override void Write(VerifyJsonWriter writer, Delegate @delegate)
    {
        writer.WriteStartObject();

        writer.WriteMember(@delegate, @delegate.GetType(), "Type");

        var declaringType = @delegate.Method.DeclaringType;
        writer.WriteMember(@delegate, declaringType, "Target");

        var s = @delegate.Method.ToString()!;
        writer.WriteMember(@delegate, CleanMethodName(s), "Method");

        writer.WriteEndObject();
    }

    static string CleanMethodName(string name)
    {
        var split = name.Split('<', '>', '(');
        if (split.Length > 2)
        {
            split[2] = "(";
            return string.Concat(split);
        }

        return name;
    }
}
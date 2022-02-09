class DelegateConverter :
    WriteOnlyJsonConverter<Delegate>
{
    public override void Write(VerifyJsonWriter writer, Delegate @delegate)
    {
        writer.WriteStartObject();

        writer.WriteProperty(@delegate, @delegate.GetType(), "Type");

        var declaringType = @delegate.Method.DeclaringType;
        writer.WriteProperty(@delegate, declaringType, "Target");

        var s = @delegate.Method.ToString()!;
        writer.WriteProperty(@delegate, CleanMethodName(s), "Method");

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
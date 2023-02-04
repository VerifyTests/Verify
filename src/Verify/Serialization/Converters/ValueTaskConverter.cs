class ValueTaskConverter :
    WriteOnlyJsonConverter
{
    static MethodInfo genericWriteDef;

    static ValueTaskConverter() =>
        genericWriteDef = typeof(ValueTaskConverter)
            .GetMethod("WriteGeneric", BindingFlags.Static | BindingFlags.NonPublic)!;

    public override bool CanConvert(Type type)
    {
        if (type == typeof(ValueTask))
        {
            return true;
        }

        if (!type.IsGenericType)
        {
            return false;
        }

        var definition = type.GetGenericTypeDefinition();
        return definition == typeof(ValueTask<>);
    }

    public override void Write(VerifyJsonWriter writer, object value)
    {
        writer.WriteStartObject();

        if (value is ValueTask task)
        {
            writer.WriteMember(task, task.IsCanceled, "IsCanceled");
            writer.WriteMember(task, task.IsCompleted, "IsCompleted");
            writer.WriteMember(task, task.IsFaulted, "IsFaulted");
        }
        else
        {
            var typeArguments = value.GetType().GetGenericArguments()[0];
            var genericWrite = genericWriteDef.MakeGenericMethod(typeArguments);
            genericWrite.Invoke(null, new[]
            {
                writer,
                value
            });
        }

        writer.WriteEndObject();
    }

    // ReSharper disable once UnusedMember.Local
    static void WriteGeneric<T>(VerifyJsonWriter writer, ValueTask<T?> task)
        where T : notnull
    {
        writer.WriteMember(task, task.IsCanceled, "IsCanceled");
        writer.WriteMember(task, task.IsCompleted, "IsCompleted");
        writer.WriteMember(task, task.IsFaulted, "IsFaulted");
        WriteResult(writer, task);
    }

    static void WriteResult<T>(VerifyJsonWriter writer, ValueTask<T?> task)
        where T : notnull
    {
        if (!task.IsCompleted ||
            task.IsCanceled ||
            task.IsFaulted)
        {
            return;
        }

        var type = task.GetType();

        if (!type.IsGenericType)
        {
            return;
        }

        writer.WriteMember(task, task.Result, "Result");
    }
}
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
            writer.WriteProperty(task, task.IsCanceled, "IsCanceled");
            writer.WriteProperty(task, task.IsCompleted, "IsCompleted");
            writer.WriteProperty(task, task.IsFaulted, "IsFaulted");
        }
        else
        {
            var typeArguments = value.GetType().GetGenericArguments().Single();
            var genericWrite = genericWriteDef.MakeGenericMethod(typeArguments);
            genericWrite.Invoke(null, new[]
            {
                writer,
                value
            });
        }

        writer.WriteEndObject();
    }

    static void WriteGeneric<T>(VerifyJsonWriter writer, ValueTask<T> task)
    {
        writer.WriteProperty(task, task.IsCanceled, "IsCanceled");
        writer.WriteProperty(task, task.IsCompleted, "IsCompleted");
        writer.WriteProperty(task, task.IsFaulted, "IsFaulted");
        WriteResult(writer, task);
    }

    static void WriteResult<T>(VerifyJsonWriter writer, ValueTask<T> task)
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

        writer.WriteProperty(task, task.Result, "Result");
    }
}
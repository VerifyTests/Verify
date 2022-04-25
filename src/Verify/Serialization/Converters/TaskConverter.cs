class TaskConverter :
    WriteOnlyJsonConverter<Task>
{
    public override void Write(VerifyJsonWriter writer, Task task)
    {
        writer.WriteStartObject();
        writer.WriteProperty(task, task.Status, "Status");

        if (task.CreationOptions != TaskCreationOptions.None)
        {
            writer.WriteProperty(task, task.CreationOptions, "CreationOptions");
        }

        writer.WriteProperty(task, task.Exception, "Exception");
        WriteResult(writer, task);

        writer.WriteEndObject();
    }

    static void WriteResult(VerifyJsonWriter writer, Task task)
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

        var name = type.GenericTypeArguments.First().Name;
        if (name == "VoidTaskResult")
        {
            return;
        }

        var resultProperty = type.GetProperty("Result")!;
        var result = resultProperty.GetValue(task);
        writer.WriteProperty(task, result, "Result");
    }
}
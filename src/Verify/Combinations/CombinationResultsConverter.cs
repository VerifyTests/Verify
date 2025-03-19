namespace VerifyTests;

public class CombinationResultsConverter :
    WriteOnlyJsonConverter<CombinationResults>
{
    public override void Write(VerifyJsonWriter writer, CombinationResults results)
    {
        writer.WriteStartObject();

        var items = results.Items;
        if (items.Count == 0)
        {
            return;
        }

        var keysLength = items[0].Keys.Count;

        int[] maxKeyLengths;
        if (results.Columns == null)
        {
            maxKeyLengths = new int[keysLength];
        }
        else
        {
            maxKeyLengths = results.Columns.Select(_=>_.Length).ToArray();
        }

        var keyValues = new string[items.Count, keysLength];

        for (var itemIndex = 0; itemIndex < items.Count; itemIndex++)
        {
            var item = items[itemIndex];
            for (var keyIndex = 0; keyIndex < keysLength; keyIndex++)
            {
                var key = item.Keys[keyIndex];
                var name = VerifierSettings.GetNameForParameter(key, writer.Counter, pathFriendly: false);
                keyValues[itemIndex, keyIndex] = name;
                var currentKeyLength = maxKeyLengths[keyIndex];
                if (name.Length > currentKeyLength)
                {
                    maxKeyLengths[keyIndex] = name.Length;
                }
            }
        }

        WriteColumns(writer, results, maxKeyLengths);

        // keys is reused
        var keys = new CombinationKey[keysLength];
        for (var itemIndex = 0; itemIndex < items.Count; itemIndex++)
        {
            for (var keyIndex = 0; keyIndex < keysLength; keyIndex++)
            {
                keys[keyIndex] = new(
                    Value: keyValues[itemIndex, keyIndex],
                    MaxLength: maxKeyLengths[keyIndex],
                    Type: results.KeyTypes?[keyIndex]);
            }

            var item = items[itemIndex];
            var name = BuildPropertyName(keys);
            writer.WritePropertyName(name);
            WriteValue(writer, item);
        }

        writer.WriteEndObject();
    }

    static void WriteColumns(VerifyJsonWriter writer, CombinationResults results, int[] maxKeyLengths)
    {
        if (results.Columns == null)
        {
            return;
        }

        var builder = new StringBuilder();
        for (var index = 0; index < results.Columns.Count; index++)
        {
            var column = results.Columns[index];
            var maxLength = maxKeyLengths[index];
            var padding = maxLength - column.Length;
            builder.Append(column);
            builder.Append(' ', padding);
            builder.Append(", ");
        }
        builder.Length -= 2;

        writer.WritePropertyName(builder.ToString());
        writer.WriteValue("Result");
    }

    protected virtual string BuildPropertyName(IReadOnlyList<CombinationKey> keys)
    {
        var builder = new StringBuilder();
        foreach (var (value, maxLength, type) in keys)
        {
            var padding = maxLength - value.Length;
            if (type != null &&
                type.IsNumeric())
            {
                builder.Append(' ', padding);
                builder.Append(value);
            }
            else
            {
                builder.Append(value);
                builder.Append(' ', padding);
            }

            builder.Append(", ");
        }

        builder.Length -= 2;
        return builder.ToString();
    }

    protected virtual void WriteValue(VerifyJsonWriter writer, CombinationResult result)
    {
        switch (result.Type)
        {
            case CombinationResultType.Void:
                writer.WriteValue("void");
                break;
            case CombinationResultType.Value:
                if (result.Value == null)
                {
                    writer.WriteNull();
                }
                else
                {
                    writer.Serialize(result.Value);
                }
                break;
            case CombinationResultType.Exception:
                var exception = result.Exception;
                var message = exception.Message;
                if (exception is ArgumentException)
                {
                    message = FlattenMessage(message);
                }

                writer.WriteValue($"{exception.GetType().Name}: {message}");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    static string FlattenMessage(string message)
    {
        var builder = new StringBuilder();

        foreach (var line in message.AsSpan().EnumerateLines())
        {
            var trimmed = line.TrimEnd();
            builder.Append(trimmed);
            if (!trimmed.EndsWith('.'))
            {
                builder.Append(". ");
            }
        }

        builder.TrimEnd();
        return builder.ToString();
    }
}
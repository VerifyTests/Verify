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

        var maxKeyLengths = new int[keysLength];
        var keyValues = new string[items.Count, keysLength];

        for (var itemIndex = 0; itemIndex < items.Count; itemIndex++)
        {
            var item = items[itemIndex];
            for (var keyIndex = 0; keyIndex < keysLength; keyIndex++)
            {
                var key = item.Keys[keyIndex];
                var name = VerifierSettings.GetNameForParameter(key, pathFriendly: false);
                keyValues[itemIndex, keyIndex] = name;
                var currentKeyLength = maxKeyLengths[keyIndex];
                if (name.Length > currentKeyLength)
                {
                    maxKeyLengths[keyIndex] = name.Length;
                }
            }
        }

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
        var exception = result.Exception;
        if (exception == null)
        {
            if(result.Value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.Serialize(result.Value);
            }

            return;
        }

        var message = exception.Message;
        if (exception is ArgumentException)
        {
            message = FlattenMessage(message);
        }

        writer.WriteValue($"{exception.GetType().Name}: {message}");
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
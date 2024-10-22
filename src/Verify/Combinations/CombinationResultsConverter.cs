using StringBuilder = System.Text.StringBuilder;

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
                var name = VerifierSettings.GetNameForParameter(key, false);
                keyValues[itemIndex, keyIndex] = name;
                var currentKeyLength = maxKeyLengths[keyIndex];
                if (name.Length > currentKeyLength)
                {
                    maxKeyLengths[keyIndex] = name.Length;
                }
            }
        }

        for (var itemIndex = 0; itemIndex < items.Count; itemIndex++)
        {
            var item = items[itemIndex];
            var builder = new StringBuilder();
            for (var keyIndex = 0; keyIndex < keysLength; keyIndex++)
            {
                var keyValue = keyValues[itemIndex, keyIndex];
                var maxKeyLength = maxKeyLengths[keyIndex];
                var keyType = results.KeyTypes?[keyIndex];
                if (keyType != null &&
                    keyType.IsNumeric())
                {
                    builder.Append(' ', maxKeyLength - keyValue.Length);
                    builder.Append(keyValue);
                }
                else
                {
                    builder.Append(keyValue);
                    builder.Append(' ', maxKeyLength - keyValue.Length);
                }

                if (keyIndex + 1 != keysLength)
                {
                    builder.Append(", ");
                }
            }

            writer.WritePropertyName(builder.ToString());
            WriteValue(writer, item);
        }

        writer.WriteEndObject();
    }

    static void WriteValue(VerifyJsonWriter writer, CombinationResult item)
    {
        var exception = item.Exception;
        if (exception == null)
        {
            writer.WriteValue(item.Value);
            return;
        }

        var message = exception.Message;
        var builder = new StringBuilder();
        var split = message.Split('\r', '\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in split)
        {
            var trimmed = line.TrimEnd();
            if (trimmed.EndsWith('.'))
            {
                builder.Append(trimmed);
            }
            else
            {
                builder.Append(trimmed);
                builder.Append(". ");
            }
        }
        builder.TrimEnd();
        writer.WriteValue($"{exception.GetType().Name}: {builder}");
    }
}
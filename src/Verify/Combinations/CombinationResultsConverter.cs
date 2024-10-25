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
            var keys = new string[keysLength];
            for (var keyIndex = 0; keyIndex < keysLength; keyIndex++)
            {
                var keyValue = keyValues[itemIndex, keyIndex];
                keys[keyIndex] = keyValue;
            }

            var item = items[itemIndex];
            var name = BuildPropertyName(results, keys, maxKeyLengths);
            writer.WritePropertyName(name);
            WriteValue(writer, item);
        }

        writer.WriteEndObject();
    }

    protected virtual string BuildPropertyName(CombinationResults results, IReadOnlyList<string> keys, IReadOnlyList<int> maxKeyLengths)
    {
        var builder = new StringBuilder();
        for (var index = 0; index < keys.Count; index++)
        {
            var key = keys[index];
            var maxLength = maxKeyLengths[index];
            var type = results.KeyTypes?[index];
            var padding = maxLength - key.Length;
            if (type != null &&
                type.IsNumeric())
            {
                builder.Append(' ', padding);
                builder.Append(key);
            }
            else
            {
                builder.Append(key);
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
            writer.WriteValue(result.Value);
            return;
        }

        writer.WriteValue($"{exception.GetType().Name}: {exception.Message}");
    }
}
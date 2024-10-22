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

        var keysLength = items.First().Keys.Length;

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
                var key = item.Keys[keyIndex];
                var maxKeyLength = maxKeyLengths[keyIndex];
                builder.Append(keyValue);
                builder.Append(' ', maxKeyLength - keyValue.Length);
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

        writer.WriteValue($"{exception.GetType().Name}: {exception.Message}");
    }
}
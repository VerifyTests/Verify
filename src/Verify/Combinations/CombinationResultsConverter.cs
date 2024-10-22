namespace VerifyTests;

public class CombinationResultsConverter :
    WriteOnlyJsonConverter<List<CombinationResult>>
{
    public override void Write(VerifyJsonWriter writer, List<CombinationResult> items)
    {
        writer.WriteStartObject();
        foreach (var item in items)
        {
            writer.WritePropertyName(BuildKeys(item.Keys).ToString());
            var exception = item.Exception;
            if (exception == null)
            {
                writer.WriteValue(item.Value);
            }
            else
            {
                writer.WriteValue($"{exception.GetType().Name}: {exception.Message}");
            }
        }
        writer.WriteEndObject();
    }

    static StringBuilder BuildKeys(object?[] combo)
    {
        var builder = new StringBuilder();
        for (var index = 0; index < combo.Length; index++)
        {
            var item = combo[index];
            VerifierSettings.AppendParameter(item, builder, true);
            if (index + 1 != combo.Length)
            {
                builder.Append(", ");
            }
        }

        return builder;
    }
}
namespace VerifyTests;

public class VerifyJsonWriter :
    JsonTextWriter
{
    StringBuilder builder;
    internal SerializationSettings settings;
    public IReadOnlyDictionary<string, object> Context { get; }
    public Counter Counter { get; }

    public VerifyJsonWriter(StringBuilder builder, SerializationSettings settings, IReadOnlyDictionary<string, object> context, Counter counter) :
        base(
            new StringWriter(builder)
            {
                NewLine = "\n"
            })
    {
        this.builder = builder;
        this.settings = settings;
        Context = context;
        Counter = counter;
        if (!VerifierSettings.StrictJson)
        {
            QuoteChar = '\'';
            QuoteName = false;
        }
    }

    public override void WriteValue(string? value)
    {
        if (value is null)
        {
            base.WriteValue(value);
            return;
        }

        value = value.Replace("\r\n", "\n").Replace('\r', '\n');
        if (VerifierSettings.StrictJson)
        {
            base.WriteValue(value);
            return;
        }

        if (value.Contains('\n'))
        {
            base.Flush();
            var builderLength = builder.Length;
            value = $"\n{value}";
            base.WriteRawValue(value);
            base.Flush();
            builder.Remove(builderLength, 1);
            return;
        }

        base.WriteRawValue(value);
    }

    public override void WriteValue(byte[]? value)
    {
        if (value is null)
        {
            WriteNull();
        }
        else
        {
            WriteValue(Convert.ToBase64String(value));
        }
    }

    public override void WriteValue(DateTimeOffset value)
    {
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            WriteValue(value.ToString("yyyy-MM-ddK", CultureInfo.InvariantCulture));
        }
        else
        {
            WriteValue(value.ToString("yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture));
        }
    }

    public override void WriteValue(DateTime value)
    {
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            WriteValue(value.ToString("yyyy-MM-ddK", CultureInfo.InvariantCulture));
        }
        else
        {
            WriteValue(value.ToString("yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture));
        }
    }

    public override void WriteValue(TimeSpan value)
    {
        WriteValue(value.ToString());
    }

    /// <summary>
    /// Writes a property name and value while respecting other custom serialization settings.
    /// </summary>
    public void WriteProperty<T, TMember>(T target, TMember value, string name)
    {
        if (settings.ShouldIgnore<T, TMember>(name))
        {
            return;
        }

        InnerWriteProperty(target, value, name);
    }

    void InnerWriteProperty<T, TMember>(T target, TMember value, string name)
    {
        if (!settings.ShouldSerialize(value))
        {
            return;
        }

        var converter = VerifierSettings.GetMemberConverter<T>(name);
        if (converter != null)
        {
            var converted = converter(target!, value);
            if (converted == null)
            {
                return;
            }

            WritePropertyName(name);
            WriteOrSerialize(converted);

            return;
        }

        WritePropertyName(name);
        WriteOrSerialize(value);
    }

    void WriteOrSerialize(object converted)
    {
        if (converted is string convertedString)
        {
            WriteValue(convertedString);
        }
        else if (converted.GetType().IsPrimitive)
        {
            WriteValue(converted);
        }
        else
        {
            settings.Serializer.Serialize(this, converted);
        }
    }

    /// <summary>
    /// Convenience method that calls <see cref="Serializer"/>.<see cref="JsonSerializer.Serialize(TextWriter,object?)"/> passing in the writer instance and <paramref name="value"/>
    /// </summary>
    public void Serialize(object value)
    {
        settings.Serializer.Serialize(this, value);
    }

    public JsonSerializer Serializer { get => settings.Serializer; }
}
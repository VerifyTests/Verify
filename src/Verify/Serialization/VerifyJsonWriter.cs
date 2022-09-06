namespace VerifyTests;

public class VerifyJsonWriter :
    JsonTextWriter
{
    StringBuilder builder;
    internal VerifySettings settings;
    internal SerializationSettings serialization;
    public IReadOnlyDictionary<string, object> Context { get; }
    public Counter Counter { get; }

    internal VerifyJsonWriter(StringBuilder builder, VerifySettings settings, Counter counter) :
        base(
            new StringWriter(builder)
            {
                NewLine = "\n"
            })
    {
        this.builder = builder;
        this.settings = settings;
        this.serialization = settings.serialization;
        Context = settings.Context;
        Counter = counter;
        if (!VerifierSettings.StrictJson)
        {
            QuoteValue = false;
            EscapeHandling = EscapeHandling.None;
            QuoteName = false;
        }
    }

    public void WriteRawValueIfNoStrict(string value)
    {
        if (VerifierSettings.StrictJson)
        {
            base.WriteValue(value);
            return;
        }

        base.WriteRawValue(value);
    }

    public void WriteRawValueWithScrubbers(string? value)
    {
        if (value is null or "")
        {
            if (VerifierSettings.StrictJson)
            {
                base.WriteValue(value);
                return;
            }

            base.WriteRawValue(value);
            return;
        }

        value = ApplyScrubbers.ApplyForPropertyValue(value, settings);
        WriteRawValueIfNoStrict(value);
    }

    public override void WriteValue(string? value)
    {
        if (value is null or "")
        {
            base.WriteRawValue(value);
            return;
        }

        if (serialization.TryConvertString(Counter, value, out var result))
        {
            WriteRawValue(result);
            return;
        }

        value = ApplyScrubbers.ApplyForPropertyValue(value, settings);
        if (VerifierSettings.StrictJson)
        {
            base.WriteValue(value);
            return;
        }

        if (value.Contains('\n'))
        {
            base.Flush();
            var builderLength = builder.Length;
            if (!value.StartsWith('\n'))
            {
                value = $"\n{value}";
            }

            WriteRawValue(value);
            base.Flush();
            builder.Remove(builderLength, 1);
            return;
        }

        WriteRawValue(value);
    }

    public void WriteSingleLineNoScrubbing(string value)
    {
        if (value is "")
        {
            base.WriteRawValue(value);
            return;
        }

        if (VerifierSettings.StrictJson)
        {
            base.WriteValue(value);
            return;
        }

        WriteRawValue(value);
    }

    public override void WriteValue(byte[]? value)
    {
        if (value is null)
        {
            WriteNull();
            return;
        }

        WriteRawValue(Convert.ToBase64String(value));
    }

    public override void WriteValue(DateTimeOffset value)
    {
        if (serialization.TryConvert(Counter, value, out var result))
        {
            WriteRawValue(result);
            return;
        }

        if (value.TimeOfDay == TimeSpan.Zero)
        {
            WriteRawValueWithScrubbers(value.ToString("yyyy-MM-ddK", CultureInfo.InvariantCulture));
            return;
        }

        WriteRawValueWithScrubbers(value.ToString("yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture));
    }

    public override void WriteValue(DateTime value)
    {
        if (serialization.TryConvert(Counter, value, out var result))
        {
            WriteRawValue(result);
            return;
        }

        if (value.TimeOfDay == TimeSpan.Zero)
        {
            WriteRawValueWithScrubbers(value.ToString("yyyy-MM-ddK", CultureInfo.InvariantCulture));
            return;
        }

        WriteRawValueWithScrubbers(value.ToString("yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture));
    }

    public override void WriteValue(TimeSpan value) =>
        WriteRawValue(value.ToString());

    public override void WriteValue(Guid value)
    {
        if (serialization.TryConvert(Counter, value, out var result))
        {
            WriteRawValue(result);
            return;
        }

        WriteRawValueWithScrubbers(value.ToString("D", CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Writes a property name and value while respecting other custom serialization settings.
    /// </summary>
    public void WriteMember(object target, object? value, string name)
    {
        if (value is null)
        {
            return;
        }

        var declaringType = target.GetType();
        var memberType = value.GetType();
        if (serialization.TryGetScrubOrIgnore(declaringType, memberType, name, out var scrubOrIgnore))
        {
            if (scrubOrIgnore == ScrubOrIgnore.Ignore)
            {
                return;
            }

            WritePropertyName(name);
            WriteRawValue("{Scrubbed}");

            return;
        }

        if (serialization.TryGetScrubOrIgnoreByInstance(value, out scrubOrIgnore))
        {
            if (scrubOrIgnore == ScrubOrIgnore.Ignore)
            {
                return;
            }

            WritePropertyName(name);
            WriteRawValue("{Scrubbed}");

            return;
        }

        var converter = VerifierSettings.GetMemberConverter(declaringType, name);
        if (converter is not null)
        {
            var converted = converter(target, value);
            if (converted is null)
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
    /// Convenience method that calls <see cref="Serializer" />.<see cref="JsonSerializer.Serialize(TextWriter,object?)" /> passing in the writer instance and <paramref name="value" />
    /// </summary>
    public void Serialize(object value) =>
        settings.Serializer.Serialize(this, value);

    public JsonSerializer Serializer => settings.Serializer;
}
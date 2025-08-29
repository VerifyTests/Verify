using Formatting = Argon.Formatting;

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
        serialization = settings.serialization;
        Context = settings.Context;
        Counter = counter;
        Formatting = Formatting.Indented;
        if (!settings.StrictJson)
        {
            QuoteValue = false;
            EscapeHandling = EscapeHandling.None;
            QuoteName = false;
        }
    }

    public void WriteRawValueIfNoStrict(string value) =>
        WriteRawValueIfNoStrict(value.AsSpan());

    public void WriteRawValueIfNoStrict(CharSpan value)
    {
        if (settings.StrictJson)
        {
            base.WriteValue(value);
            return;
        }

        base.WriteRawValue(value);
    }

    public override void WriteValue(char value)
    {
        if (settings.StrictJson)
        {
            base.WriteValue(value);
            return;
        }

        base.WriteRawValue(value.ToString());
    }

    public void WriteRawValueWithScrubbers(string value) =>
        WriteRawValueWithScrubbers(value.AsSpan());

    public void WriteRawValueWithScrubbers(CharSpan value)
    {
        if (value.Length == 0)
        {
            WriteRawValueIfNoStrict(value);
            return;
        }

        value = ApplyScrubbers.ApplyForPropertyValue(value, settings, Counter);
        WriteRawValueIfNoStrict(value);
    }

    public override void WritePropertyName(CharSpan name, bool escape)
    {
        if (settings.StrictJson)
        {
            escape = false;
        }

        base.WritePropertyName(name, escape);
    }

    public override void WritePropertyName(string name, bool escape)
    {
        if (settings.StrictJson)
        {
            escape = false;
        }

        base.WritePropertyName(name, escape);
    }

    public override void WriteValue(string? value)
    {
        if (value is null)
        {
            base.WriteNull();
            return;
        }

        WriteValue(value.AsSpan());
    }

    public override void WriteValue(StringBuilder? value) =>
        // TODO:
        WriteValue(value?.ToString());

    public override void WriteValue(CharSpan value)
    {
        if (value is "")
        {
            WriteRawValueIfNoStrict(value);
            return;
        }

        if (Counter.TryConvertString(value, out var result))
        {
            WriteRawValueIfNoStrict(result);
            return;
        }

        value = ApplyScrubbers.ApplyForPropertyValue(value, settings, Counter);
        if (settings.StrictJson)
        {
            base.WriteValue(value);
            return;
        }

        if (value.Contains('\n'))
        {
            base.Flush();
            var builderLength = builder.Length;
            if (value[0] != '\n')
            {
                WriteRawValue("\n");
                WriteRaw(value);
            }
            else
            {
                WriteRawValue(value);
            }

            base.Flush();
            builder.Remove(builderLength, 1);
            return;
        }

        WriteRawValueIfNoStrict(value);
    }

    public override void WriteValue(byte[]? value)
    {
        if (value is null)
        {
            WriteNull();
            return;
        }

        WriteRawValueIfNoStrict(Convert.ToBase64String(value));
    }

    public override void WriteValue(DateTimeOffset value)
    {
        if (Counter.TryConvert(value, out var result))
        {
            WriteRawValueIfNoStrict(result);
            return;
        }

        WriteRawValueWithScrubbers(DateFormatter.ToJsonString(value));
    }

    public override void WriteValue(DateTime value)
    {
        if (Counter.TryConvert(value, out var result))
        {
            WriteRawValueIfNoStrict(result);
            return;
        }

        WriteRawValueWithScrubbers(DateFormatter.ToJsonString(value));
    }

    public override void WriteValue(TimeSpan value) =>
        WriteRawValueIfNoStrict(value.ToString());

    public override void WriteValue(Guid value)
    {
        if (Counter.TryConvert(value, out var result))
        {
            WriteRawValueIfNoStrict(result);
            return;
        }

        Span<char> buffer = stackalloc char[36];
        value.TryFormat(buffer, out _);
        WriteRawValueWithScrubbers(buffer);
    }

    /// <summary>
    /// Writes a property name and value while respecting other custom serialization settings.
    /// </summary>
    public void WriteMember<T>(object target, T? value, string name, T defaultIgnore)
    {
        if (value is null)
        {
            return;
        }

        if (EqualityComparer<T>.Default.Equals(value, defaultIgnore))
        {
            return;
        }

        WriteMember(target, value, name);
    }

    /// <summary>
    /// Writes a property name and value while respecting other custom serialization settings.
    /// </summary>
    public void WriteMember(object target, object? value, string name)
    {
        if (value is null)
        {
            WriteNullMember(name);
            return;
        }

        if (ReferenceEquals(target, value))
        {
            WriteRawOrStrictMember(name, "$parentValue");
            return;
        }

        var declaringType = target.GetType();
        var memberType = value.GetType();
        if (serialization.TryGetScrubOrIgnore(declaringType, memberType, name, out var scrubOrIgnore))
        {
            if (scrubOrIgnore != ScrubOrIgnore.Ignore)
            {
                WriteRawOrStrictMember(name, "Scrubbed");
            }

            return;
        }

        if (serialization.TryGetScrubOrIgnoreByInstance(value, out scrubOrIgnore))
        {
            if (scrubOrIgnore != ScrubOrIgnore.Ignore)
            {
                WriteRawOrStrictMember(name, "Scrubbed");
            }

            return;
        }

        var converter = VerifierSettings.GetMemberConverter(declaringType, name);
        if (converter is not null)
        {
            value = converter(target, value);
            if (value is null)
            {
                return;
            }
        }

        WritePropertyName(name);
        WriteOrSerialize(value);
    }

    /// <summary>
    /// Writes a property name and value while respecting other custom serialization settings.
    /// </summary>
    public void WriteMember(object target, string? value, string name)
    {
        if (value is null)
        {
            WriteNullMember(name);
            return;
        }

        var declaringType = target.GetType();
        if (serialization.TryGetScrubOrIgnore(declaringType, typeof(string), name, out var scrubOrIgnore))
        {
            if (scrubOrIgnore != ScrubOrIgnore.Ignore)
            {
                WriteRawOrStrictMember(name, "Scrubbed");
            }

            return;
        }

        if (serialization.TryGetScrubOrIgnoreByInstance(value, out scrubOrIgnore))
        {
            if (scrubOrIgnore != ScrubOrIgnore.Ignore)
            {
                WriteRawOrStrictMember(name, "Scrubbed");
            }

            return;
        }

        var converter = VerifierSettings.GetMemberConverter(declaringType, name);
        if (converter is not null)
        {
            value = (string?) converter(target, value);
            if (value is null)
            {
                return;
            }
        }

        WritePropertyName(name);
        WriteValue(value);
    }

    /// <summary>
    /// Writes a property name and value while respecting other custom serialization settings.
    /// </summary>
    public void WriteMember(object target, CharSpan value, string name)
    {
        var declaringType = target.GetType();
        if (serialization.TryGetScrubOrIgnore(declaringType, typeof(CharSpan), name, out var scrubOrIgnore))
        {
            if (scrubOrIgnore != ScrubOrIgnore.Ignore)
            {
                WriteRawOrStrictMember(name, "Scrubbed");
            }

            return;
        }

        //TODO: support instance scrubbing for CharSpan?
        // if (serialization.TryGetScrubOrIgnoreByInstance(value, out scrubOrIgnore))
        // {
        //     if (scrubOrIgnore != ScrubOrIgnore.Ignore)
        //     {
        //         WriteRawOrStrictMember(name, "Scrubbed");
        //     }
        //
        //     return;
        // }

        //TODO: support converters for CharSpan?
        // var converter = VerifierSettings.GetMemberConverter(declaringType, name);
        // if (converter is not null)
        // {
        //     value = (string?) converter(target, value);
        //     if (value is null)
        //     {
        //         return;
        //     }
        // }

        WritePropertyName(name);
        WriteValue(value);
    }

    void WriteRawOrStrictMember(string name, string value)
    {
        WritePropertyName(name);
        WriteRawValueIfNoStrict(value);
    }

    void WriteNullMember(string name)
    {
        if (serialization.TryGetScrubOrIgnoreByName(name, out var scrubOrIgnoreByName))
        {
            if (scrubOrIgnoreByName != ScrubOrIgnore.Ignore)
            {
                WriteRawOrStrictMember(name, "Scrubbed");
            }
        }
        else if (!serialization.IgnoreNulls)
        {
            WritePropertyName(name);
            WriteNull();
        }
    }

    void WriteOrSerialize(object value)
    {
        if (value is string stringValue)
        {
            WriteValue(stringValue);
            return;
        }

        if (value.GetType().IsPrimitive)
        {
            WriteValue(value);
            return;
        }

        settings.Serializer.Serialize(this, value);
    }

    /// <summary>
    /// Convenience method that calls <see cref="Serializer" />.<see cref="JsonSerializer.Serialize(TextWriter,object?)" /> passing in the writer instance and <paramref name="value" />
    /// </summary>
    public void Serialize(object value) =>
        settings.Serializer.Serialize(this, value);

    public JsonSerializer Serializer => settings.Serializer;
}
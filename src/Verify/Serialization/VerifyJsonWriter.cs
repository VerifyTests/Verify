using System.Globalization;
using System.Linq.Expressions;
using Newtonsoft.Json;
namespace VerifyTests;

public class VerifyJsonWriter :
    JsonTextWriter
{
    StringBuilder builder;
    SerializationSettings settings;
    public IReadOnlyDictionary<string, object> Context { get; }

    public VerifyJsonWriter(StringBuilder builder, SerializationSettings settings, IReadOnlyDictionary<string, object> context) :
        base(
            new StringWriter(builder)
            {
                NewLine = "\n"
            })
    {
        this.builder = builder;
        this.settings = settings;
        Context = context;
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

        if (value.Contains("\n"))
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

    public void WriteProperty<T, TMember>(T target, Expression<Func<T, TMember>> expression)
    {
        var member = expression.FindMember();
        if (settings.ShouldIgnore(member))
        {
            return;
        }

        var value = expression.Compile().Invoke(target);
        if (!settings.ShouldSerialize(value))
        {
            return;
        }

        var converter = VerifierSettings.GetMemberConverter(member);
        if (converter != null)
        {
            var converted = converter(target!, value);
            if (converted == null)
            {
                return;
            }

            WritePropertyName(member.Name);
            WriteOrSerialize(converted);

            return;
        }

        WritePropertyName(member.Name);
        WriteOrSerialize(value!);
    }

    void WriteOrSerialize(object converted)
    {
        if (converted is string convertedString)
        {
            WriteRawValue(convertedString);
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
}
partial class CustomContractResolver
{
    protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
    {
        var contract = base.CreateDictionaryContract(objectType);
        if (settings.OrderDictionaries)
        {
            contract.OrderByKey = true;
        }

        contract.InterceptSerializeItem = (writer, o, value) => HandleDictionaryItem((VerifyJsonWriter) writer, o, value);

        return contract;
    }

    KeyValueInterceptResult HandleDictionaryItem(VerifyJsonWriter writer, object key, object? value)
    {
        if (key is string stringKey &&
            settings.TryGetScrubOrIgnoreByName(stringKey, out var scrubOrIgnore))
        {
            return ToInterceptKeyValueResult(scrubOrIgnore.Value);
        }

        if (value is not null &&
            settings.TryGetScrubOrIgnoreByInstance(value, out scrubOrIgnore))
        {
            return ToInterceptKeyValueResult(scrubOrIgnore.Value);
        }

        if (TryConvertDictionaryKey(writer, key, out var resultKey))
        {
            return KeyValueInterceptResult.ReplaceKey(resultKey);
        }

        return KeyValueInterceptResult.Default;
    }

    static KeyValueInterceptResult ToInterceptKeyValueResult(ScrubOrIgnore scrubOrIgnore)
    {
        if (scrubOrIgnore == ScrubOrIgnore.Ignore)
        {
            return KeyValueInterceptResult.Ignore;
        }

        return KeyValueInterceptResult.ReplaceValue("{Scrubbed}");
    }

    static bool TryConvertDictionaryKey(VerifyJsonWriter writer, object original, [NotNullWhen(true)] out string? result)
    {
        var counter = writer.Counter;

#if NET6_0_OR_GREATER

        if (original is Date date)
        {
            if (counter.TryConvert(date, out result))
            {
                return true;
            }
        }

        if (original is Time time)
        {
            if (counter.TryConvert(time, out result))
            {
                return true;
            }
        }

#endif

        if (original is Guid guid)
        {
            if (counter.TryConvert(guid, out result))
            {
                return true;
            }
        }

        if (original is DateTime dateTime)
        {
            if (counter.TryConvert(dateTime, out result))
            {
                return true;
            }
        }

        if (original is DateTimeOffset dateTimeOffset)
        {
            if (counter.TryConvert(dateTimeOffset, out result))
            {
                return true;
            }
        }

        if (original is string stringValue)
        {
            if (counter.TryParseConvert(stringValue.AsSpan(), out result))
            {
                return true;
            }

            result = ApplyScrubbers.ApplyForPropertyValue(stringValue.AsSpan(), writer.settings, counter).ToString();

            return true;
        }

        if (original is Type type)
        {
            result = type.SimpleName();
            return true;
        }

        result = null;
        return false;
    }
}
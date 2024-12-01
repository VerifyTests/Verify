partial class CustomContractResolver
{
    protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
    {
        var contract = base.CreateDictionaryContract(objectType);
        if (settings.OrderDictionaries)
        {
            contract.OrderByKey = true;
        }

        contract.InterceptSerializeItem = HandleDictionaryItem;

        return contract;
    }

    KeyValueInterceptResult HandleDictionaryItem(JsonWriter writer, object key, object? value)
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

    bool TryConvertDictionaryKey(JsonWriter writer, object original, [NotNullWhen(true)] out string? result)
    {
        var counter = Counter.Current;

#if NET6_0_OR_GREATER

        if (original is Date date)
        {
            if (settings.TryConvert(counter, date, out result))
            {
                return true;
            }
        }

        if (original is Time time)
        {
            if (settings.TryConvert(counter, time, out result))
            {
                return true;
            }
        }

#endif

        if (original is Guid guid)
        {
            if (settings.TryConvert(counter, guid, out result))
            {
                return true;
            }
        }

        if (original is string stringValue)
        {
            if (settings.TryParseConvert(counter, stringValue.AsSpan(), out result))
            {
                return true;
            }

            var verifyJsonWriter = (VerifyJsonWriter)writer;
            result = ApplyScrubbers.ApplyForPropertyValue(stringValue.AsSpan(), verifyJsonWriter.settings, counter).ToString();

            return true;
        }

        if (original is DateTime dateTime)
        {
            if (settings.TryConvert(counter, dateTime, out result))
            {
                return true;
            }
        }

        if (original is DateTimeOffset dateTimeOffset)
        {
            if (settings.TryConvert(counter, dateTimeOffset, out result))
            {
                return true;
            }
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
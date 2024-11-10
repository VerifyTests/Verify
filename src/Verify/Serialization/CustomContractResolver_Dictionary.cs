partial class CustomContractResolver
{
    protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
    {
        var contract = base.CreateDictionaryContract(objectType);
        contract.DictionaryKeyResolver = (_, name, original) => ResolveDictionaryKey(name, original);
        if (settings.SortDictionaries)
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

    string ResolveDictionaryKey(string name, object original)
    {
        var counter = Counter.Current;

#if NET6_0_OR_GREATER

        if (original is Date date)
        {
            if (settings.TryConvert(counter, date, out var result))
            {
                return result;
            }
        }

        if (original is Time time)
        {
            if (settings.TryConvert(counter, time, out var result))
            {
                return result;
            }
        }

#endif

        if (original is Guid guid)
        {
            if (settings.TryConvert(counter, guid, out var result))
            {
                return result;
            }
        }

        if (original is string stringValue)
        {
            if (settings.TryParseConvert(counter, stringValue.AsSpan(), out var result))
            {
                return result;
            }
        }

        if (original is DateTime dateTime)
        {
            if (settings.TryConvert(counter, dateTime, out var result))
            {
                return result;
            }
        }

        if (original is DateTimeOffset dateTimeOffset)
        {
            if (settings.TryConvert(counter, dateTimeOffset, out var result))
            {
                return result;
            }
        }

        if (original is Type type)
        {
            return type.SimpleName();
        }

        return name;
    }
}
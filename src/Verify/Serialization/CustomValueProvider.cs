class CustomValueProvider(
    IValueProvider inner,
    Type type,
    Func<Exception, bool> ignoreException,
    IEnumerable<ConvertTargetMember> converters,
    SerializationSettings settings) :
    IValueProvider
{
    public void SetValue(object target, object? value) =>
        throw new NotImplementedException();

    public object? GetValue(object target)
    {
        var hasConverter = false;
        var value = inner.GetValue(target);
        foreach (var converter in converters)
        {
            value = converter(target, value);
            hasConverter = true;
        }

        if (hasConverter)
        {
            return value;
        }

        try
        {
            if (value is not null &&
                settings.TryGetScrubOrIgnoreByInstance(value, out var scrubOrIgnore))
            {
                if (scrubOrIgnore == ScrubOrIgnore.Ignore)
                {
                    return null;
                }

                if (scrubOrIgnore == ScrubOrIgnore.Scrub)
                {
                    return "{Scrubbed}";
                }
            }

            return value;
        }
        catch (Exception exception)
        {
            var innerException = exception.InnerException;
            if (innerException is null)
            {
                throw;
            }

            if (ignoreException(innerException))
            {
                return GetDefault();
            }

            throw;
        }
    }

    object? GetDefault()
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }

        return null;
    }
}
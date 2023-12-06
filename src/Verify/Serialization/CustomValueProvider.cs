class CustomValueProvider(
    IValueProvider inner,
    Type type,
    Func<Exception, bool> ignoreException,
    ConvertTargetMember? converter,
    SerializationSettings settings) :
    IValueProvider
{
    public void SetValue(object target, object? value) =>
        throw new NotImplementedException();

    public object? GetValue(object target)
    {
        if (converter is not null)
        {
            var value = inner.GetValue(target);
            return converter(target, value);
        }

        try
        {
            var value = inner.GetValue(target);
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
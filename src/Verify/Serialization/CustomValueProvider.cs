class CustomValueProvider :
    IValueProvider
{
    IValueProvider inner;
    Type memberType;
    Func<Exception, bool> shouldIgnoreException;
    ConvertTargetMember? membersConverter;
    SerializationSettings settings;

    public CustomValueProvider(IValueProvider inner,
        Type memberType,
        Func<Exception, bool> shouldIgnoreException,
        ConvertTargetMember? membersConverter,
        SerializationSettings settings)
    {
        this.inner = inner;
        this.memberType = memberType;
        this.shouldIgnoreException = shouldIgnoreException;
        this.membersConverter = membersConverter;
        this.settings = settings;
    }

    public void SetValue(object target, object? value) =>
        throw new NotImplementedException();

    public object? GetValue(object target)
    {
        if (membersConverter is not null)
        {
            var value = inner.GetValue(target);
            return membersConverter(target, value);
        }

        try
        {
            var value = inner.GetValue(target);
            if (value != null &&
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

            if (shouldIgnoreException(innerException))
            {
                return GetDefault();
            }

            throw;
        }
    }

    object? GetDefault()
    {
        if (memberType.IsValueType)
        {
            return Activator.CreateInstance(memberType);
        }

        return null;
    }
}
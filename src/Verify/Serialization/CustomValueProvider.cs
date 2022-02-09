class CustomValueProvider :
    IValueProvider
{
    IValueProvider inner;
    Type memberType;
    IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow;
    ConvertTargetMember? membersConverter;

    public CustomValueProvider(
        IValueProvider inner,
        Type memberType,
        IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow,
        ConvertTargetMember? membersConverter)
    {
        this.inner = inner;
        this.memberType = memberType;
        this.ignoreMembersThatThrow = ignoreMembersThatThrow;
        this.membersConverter = membersConverter;
    }

    public void SetValue(object target, object? value)
    {
        throw new NotImplementedException();
    }

    public object? GetValue(object target)
    {
        if (membersConverter is not null)
        {
            var value = inner.GetValue(target);
            return membersConverter(target, value);
        }
        try
        {
            return inner.GetValue(target);
        }
        catch (Exception exception)
        {
            var innerException = exception.InnerException;
            if (innerException is null)
            {
                throw;
            }

            foreach (var func in ignoreMembersThatThrow)
            {
                if (func(innerException))
                {
                    return GetDefault();
                }
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
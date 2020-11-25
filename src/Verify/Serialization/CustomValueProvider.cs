﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

class CustomValueProvider :
    IValueProvider
{
    IValueProvider inner;
    Type propertyType;
    IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow;
    Func<object?, object?>? membersConverter;

    public CustomValueProvider(
        IValueProvider inner,
        Type propertyType,
        IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow,
        Func<object?, object?>? membersConverter)
    {
        this.inner = inner;
        this.propertyType = propertyType;
        this.ignoreMembersThatThrow = ignoreMembersThatThrow;
        this.membersConverter = membersConverter;
    }

    public void SetValue(object target, object? value)
    {
        throw new NotImplementedException();
    }

    public object? GetValue(object target)
    {
        if (membersConverter != null)
        {
            var value = inner.GetValue(target);
            return membersConverter(value);
        }
        try
        {
            return inner.GetValue(target);
        }
        catch (Exception exception)
        {
            var innerException = exception.InnerException;
            if (innerException == null)
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
        if (propertyType.IsValueType)
        {
            return Activator.CreateInstance(propertyType);
        }

        return null;
    }
}
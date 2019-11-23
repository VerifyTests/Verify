using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace ObjectApproval
{
    public class CustomValueProvider : IValueProvider
    {
        IValueProvider inner;
        Type propertyType;
        IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow;

        public CustomValueProvider(IValueProvider inner, Type propertyType, IReadOnlyList<Func<Exception, bool>> ignoreMembersThatThrow)
        {
            Guard.AgainstNull(inner, nameof(inner));
            Guard.AgainstNull(propertyType, nameof(propertyType));
            Guard.AgainstNull(ignoreMembersThatThrow, nameof(ignoreMembersThatThrow));
            this.inner = inner;
            this.propertyType = propertyType;
            this.ignoreMembersThatThrow = ignoreMembersThatThrow;
        }

        public void SetValue(object target, object? value)
        {
            throw new NotImplementedException();
        }

        public object? GetValue(object target)
        {
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
}
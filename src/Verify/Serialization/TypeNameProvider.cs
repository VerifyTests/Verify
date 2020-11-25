using System;
using Newtonsoft.Json.Serialization;

class TypeNameProvider :
    IValueProvider
{
    Type type;

    public TypeNameProvider(Type type)
    {
        this.type = type;
    }

    public void SetValue(object target, object? value)
    {
        throw new NotImplementedException();
    }

    public object? GetValue(object target)
    {
        return type.Name;
    }
}
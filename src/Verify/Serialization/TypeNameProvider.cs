class TypeNameProvider(Type type) :
    IValueProvider
{
    public void SetValue(object target, object? value) =>
        throw new NotImplementedException();

    public object? GetValue(object target) =>
        type.Name;
}
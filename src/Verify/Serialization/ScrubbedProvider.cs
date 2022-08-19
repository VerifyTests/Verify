class ScrubbedProvider : IValueProvider
{
    public void SetValue(object target, object? value) =>
        throw new NotImplementedException();

    public object GetValue(object target) =>
        "{Scrubbed}";
}
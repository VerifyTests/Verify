class NumericIdScrubProvider(IValueProvider inner) :
    IValueProvider
{
    public void SetValue(object target, object? value) =>
        throw new NotImplementedException();

    public object GetValue(object target)
    {
        var value = inner.GetValue(target);
        if (value is null)
        {
            return "Id_null";
        }

        var longValue = Convert.ToInt64(value);
        return Counter.Current.NextNumericIdString(longValue);
    }
}

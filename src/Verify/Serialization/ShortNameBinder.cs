class ShortNameBinder :
    ISerializationBinder
{
    public static readonly ShortNameBinder Instance = new();

    ShortNameBinder()
    {
    }

    public void BindToName(Type type, out string? assemblyName, out string? typeName)
    {
        assemblyName = null;
        if (type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(ArrayWrapper<>))
        {
            typeName = type.GenericTypeArguments.Single().SimpleName();
            return;
        }

        typeName = type.SimpleName();
    }

    public Type BindToType(string? assemblyName, string typeName) =>
        throw new();
}
// ReSharper disable UseObjectOrCollectionInitializer

partial static class VerifierSettings
{
    public static void AddConverter<T>()
        where T : JsonConverter, new() =>
        Converters.Add(new T());

    public static void AddConverter(JsonConverter converter) =>
        Converters.Add(converter);

    public static void AddConverterIfNotExists(JsonConverter converter)
    {
        if (Converters.Any(_ => _.GetType() == converter.GetType()))
        {
            return;
        }

        Converters.Add(converter);
    }

    internal static List<JsonConverter> Converters = new()
    {
        new AggregateExceptionConverter(),
        new StringBuilderConverter(),
        new StringWriterConverter(),
#if NET6_0_OR_GREATER
        new DateConverter(),
        new TimeConverter(),
#endif
        new FileInfoConverter(),
        new DirectoryInfoConverter(),
        new StringEnumConverter(),
        new ExpressionConverter(),
        new DelegateConverter(),
        new TargetInvocationExceptionConverter(),
        new VersionConverter(),
        new EncodingConverter(),
        new TypeJsonConverter(),
        new MethodInfoConverter(),
        new FieldInfoConverter(),
        new ConstructorInfoConverter(),
        new PropertyInfoConverter(),
        new ParameterInfoConverter(),
        new ClaimConverter(),
        new ClaimsIdentityConverter(),
        new TaskConverter(),
        new ValueTaskConverter(),
        new ClaimsPrincipalConverter(),
        new ArgonJArrayConverter(),
        new ArgonJObjectConverter(),
        new NewtonsoftJArrayConverter(),
        new NewtonsoftJObjectConverter(),
        new NameValueCollectionConverter()
    };
}
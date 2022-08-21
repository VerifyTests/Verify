[UsesVerify]
public class AddExtraSettingsTests
{
    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.AddConverter<SingleUseConverter>();

    [Fact]
    public Task Test1() =>
        Verify(new
            {
                Property = "Value2"
            })
            .AddExtraSettings(_ => _.TypeNameHandling = TypeNameHandling.All);

    [Fact]
    public Task Test2() =>
        Verify(new
            {
                Property = "Value2"
            })
            .AddExtraSettings(_ => _.TypeNameHandling = TypeNameHandling.All);

    class SingleUseConverter : JsonConverter
    {
        static bool used;

        public SingleUseConverter()
        {
            if (used)
            {
                throw new("Should only be constructed once");
            }

            used = true;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            throw new NotImplementedException();

        public override object? ReadJson(JsonReader reader, Type type, object? existingValue, JsonSerializer serializer) =>
            throw new NotImplementedException();

        public override bool CanConvert(Type type) =>
            false;
    }
}


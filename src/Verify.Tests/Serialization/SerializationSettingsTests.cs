public class SerializationSettingsTests
{
    [Fact]
    public void ExtraConverterOverridesBuiltIn()
    {
        var converter = new CustomEnumConverter();
        var settings = new SerializationSettings();
        settings.AddExtraSettings(_ => _.Converters.Add(converter));

        AssertUsesConverter(settings, converter);
        AssertUsesConverter(new(settings), converter);
    }

    [Fact]
    public void ExtraSettingsPreserveOrder()
    {
        var first = new CustomEnumConverter();
        var second = new CustomEnumConverter();
        var settings = new SerializationSettings();
        settings.AddExtraSettings(
            _ =>
            {
                _.Converters.Add(first);
                _.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            });
        settings.AddExtraSettings(
            _ =>
            {
                Assert.Equal(ReferenceLoopHandling.Serialize, _.ReferenceLoopHandling);
                _.Converters.Add(second);
                _.ReferenceLoopHandling = ReferenceLoopHandling.Error;
            });

        AssertOrder(settings, first, second);
        AssertOrder(new(settings), first, second);
    }

    [Fact]
    public void ExplicitConverterPositionIsPreserved()
    {
        var first = new CustomEnumConverter();
        var second = new CustomEnumConverter();
        var settings = new SerializationSettings();
        settings.AddExtraSettings(_ => _.Converters.Add(first));
        settings.AddExtraSettings(_ => _.Converters.Insert(0, second));

        AssertOrder(settings, second, first);
        AssertOrder(new(settings), second, first);
    }

    [Fact]
    public void RemovedConverterDoesNotAffectPriority()
    {
        var removed = new CustomEnumConverter();
        var first = new CustomEnumConverter();
        var second = new CustomEnumConverter();
        var settings = new SerializationSettings();
        settings.AddExtraSettings(_ => _.Converters.Add(removed));
        settings.AddExtraSettings(_ => _.Converters.Add(first));
        settings.AddExtraSettings(
            _ =>
            {
                _.Converters.Remove(removed);
                _.Converters.Add(second);
            });

        AssertOrder(settings, first, second);
        AssertOrder(new(settings), first, second);
    }

    static void AssertOrder(SerializationSettings settings, JsonConverter first, JsonConverter second)
    {
        Assert.Same(first, settings.Serializer.Converters[0]);
        Assert.Same(second, settings.Serializer.Converters[1]);
    }

    static void AssertUsesConverter(SerializationSettings settings, JsonConverter converter)
        => Assert.Same(converter, settings.Serializer.Converters[0]);

    class CustomEnumConverter :
        WriteOnlyJsonConverter<TestEnum>
    {
        public override void Write(VerifyJsonWriter writer, TestEnum value) =>
            writer.WriteValue("custom");
    }

    enum TestEnum
    {
        Value
    }
}

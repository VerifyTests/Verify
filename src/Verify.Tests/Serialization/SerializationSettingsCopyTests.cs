public class SerializationSettingsCopyTests
{
    [Fact]
    public void PreservesOrderDictionaries()
    {
        var settings = new SerializationSettings
        {
            OrderDictionaries = false
        };

        var copy = new SerializationSettings(settings);

        Assert.False(copy.OrderDictionaries);
    }
}

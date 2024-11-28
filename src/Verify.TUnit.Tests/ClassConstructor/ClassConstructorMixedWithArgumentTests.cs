[ClassConstructor<ClassConstructor>]
[Arguments("2")]
public class ClassConstructorMixedWithArgumentTests(string service)
{
    [Test]
    public async Task Test()
    {
        await Assert.That(service).IsNotNull();
        await Verify(service);
    }
}
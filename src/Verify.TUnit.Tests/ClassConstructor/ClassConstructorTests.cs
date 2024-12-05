[ClassConstructor<ClassConstructor>]
public class ClassConstructorTests(string service)
{
    [Test]
    public async Task Test()
    {
        await Assert.That(service).IsNotNull();
        await Verify(service);
    }
}
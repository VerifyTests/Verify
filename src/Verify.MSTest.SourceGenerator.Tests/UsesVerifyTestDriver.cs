namespace VerifyMSTest.SourceGenerator.Tests;

class UsesVerifyTestDriver : TestDriver
{
    public UsesVerifyTestDriver() : base([new UsesVerifyGenerator().AsSourceGenerator()])
    {
    }
}

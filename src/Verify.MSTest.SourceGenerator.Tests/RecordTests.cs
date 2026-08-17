// A record is a class, so it is a legal MSTest test class
[TestClass]
public partial class RecordTests : TestBase
{
    [TestMethod]
    public Task HasAttributeOnRecord()
    {
        var source = """
            using VerifyMSTest;

            namespace Foo;

            [UsesVerify]
            public partial record Bar
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [TestMethod]
    public Task HasAttributeOnRecordClass()
    {
        var source = """
            using VerifyMSTest;

            namespace Foo;

            [UsesVerify]
            public partial record class Bar
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    // A record struct is not a class, and [UsesVerify] is only valid on a class,
    // so nothing is generated for one
    [TestMethod]
    public Task HasAttributeOnRecordStruct()
    {
        var source = """
            using VerifyMSTest;

            namespace Foo;

            [UsesVerify]
            public partial record struct Bar
            {
            }
            """;

        return VerifyGenerator(
            TestDriver.Run(source),
            expectedDiagnostics: ["CS0592"]);
    }

    // The partial declaration has to repeat the kind of every parent too
    [TestMethod]
    public Task HasAttributeOnClassNestedInRecords()
    {
        var source = """
            using VerifyMSTest;

            namespace Foo;

            public partial record Outer
            {
                public partial record struct Middle
                {
                    [UsesVerify]
                    public partial class Bar
                    {
                    }
                }
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }
}

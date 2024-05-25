public class GlobalNamespaceTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public Task HasAttributeOnClass()
    {
        var source = """
            using VerifyMSTest;

            [UsesVerify]
            public partial class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [Fact]
    public Task HasAssemblyAttribute()
    {
        var source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using VerifyMSTest;

            [assembly: UsesVerify]

            [TestClass]
            public partial class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [Fact]
    public Task HasBothAssemblyAndClassAttributes()
    {
        var source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using VerifyMSTest;

            [assembly: UsesVerify]

            [TestClass]
            [UsesVerify]
            public partial class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }


    [Fact]
    public Task HasAssemblyAttributeAndCustomTestClassAttribute()
    {
        var source = """
            using System;
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using VerifyMSTest;

            [assembly: UsesVerify]

            [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
            sealed class MyTestClassAttribute : TestClassAttribute {}

            [MyTestClass]
            public partial class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }
}

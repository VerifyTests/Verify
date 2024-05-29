public class NoMatchTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public Task NoAttributes()
    {
        var source = """
            public class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [Fact]
    public Task ClassAttributeFromWrongNamespace()
    {
        var source = """
            using System;

            [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
            public sealed class UsesVerifyAttribute : Attribute {}

            [UsesVerify]
            public class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [Fact]
    public Task AssemblyAttributeFromWrongNamespace()
    {
        var source = """
            using System;
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            [assembly: UsesVerify]

            [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
            public sealed class UsesVerifyAttribute : Attribute {}

            [TestClass]
            public class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [Fact]
    public Task AssemblyAttributeButNoTestClass()
    {
        var source = """
            using VerifyMSTest;

            [assembly: UsesVerify]

            public partial class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [Fact]
    public Task TestClassButNoAssemblyAttribute()
    {
        var source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            [TestClass]
            public partial class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }
}

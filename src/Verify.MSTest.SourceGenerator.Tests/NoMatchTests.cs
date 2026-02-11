[TestClass]
public partial class NoMatchTests : TestBase
{
    [TestMethod]
    public Task NoAttributes()
    {
        var source = """
            public class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

[TestClass]
public partial class StaticClassTests : TestBase
{
    /// <summary>
    /// The shape the docs recommend for an assembly wide teardown. A static class has no instance
    /// for the generated TestContext property to live on, so emitting it is CS0708, and the
    /// generated partial declaration is CS0260 against a class that is not partial. Combined with
    /// the documented assembly wide opt in, that broke the build.
    /// </summary>
    [TestMethod]
    public Task AssemblyAttributeAndStaticTestClass()
    {
        var source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using VerifyMSTest;

            [assembly: UsesVerify]

            [TestClass]
            public static class Foo
            {
                [AssemblyCleanup]
                public static void Cleanup()
                {
                }
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    /// <summary>
    /// Declaring the class partial resolves CS0260, which leaves the property itself as the reason
    /// a static class cannot be generated for.
    /// </summary>
    [TestMethod]
    public Task AssemblyAttributeAndStaticPartialTestClass()
    {
        var source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using VerifyMSTest;

            [assembly: UsesVerify]

            [TestClass]
            public static partial class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    /// <summary>
    /// UsesVerifyAttribute permits AttributeTargets.Class, which includes a static one, so the
    /// marker path needs the same treatment as the assembly path.
    /// </summary>
    [TestMethod]
    public Task MarkerAttributeOnStaticClass()
    {
        var source = """
            using VerifyMSTest;

            [UsesVerify]
            public static partial class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    /// <summary>
    /// A static class is a legal container for a test class. Skipping static classes must skip the
    /// container only: the test class inside it is generated as usual. The container has to be
    /// partial for that, which is the generator's existing requirement of any parent type.
    /// </summary>
    [TestMethod]
    public Task TestClassNestedInStaticClass()
    {
        var source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using VerifyMSTest;

            [assembly: UsesVerify]

            public static partial class Outer
            {
                [TestClass]
                public partial class Inner
                {
                }
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }
}

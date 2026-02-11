[TestClass]
public partial class InheritanceTests : TestBase
{
    [TestMethod]
    public Task HasAttributeOnBaseClass()
    {
        var source = """
            using VerifyMSTest;

            [UsesVerify]
            public partial class Base
            {
            }

            public class Derived : Base
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [TestMethod]
    public Task HasAttributeOnDerivedClass()
    {
        var source = """
            using VerifyMSTest;

            public class Base
            {
            }

            [UsesVerify]
            public partial class Derived : Base
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [TestMethod]
    public Task HasAttributeOnBaseAndDerivedClasses()
    {
        var source = """
            using VerifyMSTest;

            [UsesVerify]
            public partial class Base
            {
            }

            [UsesVerify]
            public partial class Derived : Base
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [TestMethod]
    public Task HasAttributeOnDerivedClassAndPropertyManuallyDefinedInBase()
    {
        var source = """
            using VerifyMSTest;
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            public class Base
            {
                public TestContext TestContext { get; set; } = null!;
            }

            [UsesVerify]
            public partial class Derived : Base
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source), ["CS0506"]);
    }

    [TestMethod]
    public Task HasAttributeOnDerivedClassAndVirtualPropertyManuallyDefinedInBase()
    {
        var source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using VerifyMSTest;

            public class Base
            {
                public virtual TestContext TestContext { get; set; }
            }

            [TestClass]
            [UsesVerify]
            public partial class Derived : Base
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [TestMethod]
    public Task HasAttributeOnDerivedClassAndAbstractPropertyManuallyDefinedInBase()
    {
        var source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using VerifyMSTest;

            public abstract class Base
            {
                public abstract TestContext TestContext { get; set; }
            }

            [TestClass]
            [UsesVerify]
            public partial class Derived : Base
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [TestMethod]
    public Task HasAssemblyAttributeAndTestClassInheritance()
    {
        var source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using VerifyMSTest;

            [assembly: UsesVerify]

            [TestClass]
            public partial class Base
            {
            }

            [TestClass]
            public class Derived : Base
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [TestMethod]
    public Task HasAssemblyAttributeWithTestClassOnDerivedAndMarkerAttributeOnBase()
    {
        var source = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;
            using VerifyMSTest;

            [assembly: UsesVerify]

            [UsesVerify]
            public partial class Base
            {
            }

            [TestClass]
            public class Derived : Base
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }
}

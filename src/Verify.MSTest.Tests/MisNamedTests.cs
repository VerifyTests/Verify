//using System;
//using System.Threading.Tasks;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using VerifyTests;
//using VerifyMSTest;

//[TestClass]
//public class NoAttributeTests
//{
//    public TestContext TestContext { get; set; } = null!;

//    [TestMethod]
//    public async Task ShouldThrow()
//    {
//        var exception = await Assert.ThrowsExceptionAsync<Exception>(() => Verifier.Verify("Foo"));
//        VerifySettings settings = new();
//        settings.UseTestContext(TestContext);
//        await Verifier.Verify(exception);
//    }
//}

//[TestClass]
//public class WithContextTests
//{
//    public TestContext TestContext { get; set; } = null!;

//    [TestMethod]
//    public Task ShouldPass()
//    {
//        VerifySettings settings = new();
//        settings.UseTestContext(TestContext);
//        return Verifier.Verify("Foo", settings);
//    }
//}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnitTestProject1;

[TestClass]
public class UnitTest1 : VerifyBase
{

    [ClassInitialize]
    public static void Init(TestContext context)
        => VerifyWinForms.Enable();

    [TestMethod]
    public Task TestMethod2()
    {
        var b = new Button();
        return Verify(b).AutoVerify();
    }
}
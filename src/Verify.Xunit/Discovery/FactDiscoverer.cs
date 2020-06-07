using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;
using Options=Xunit.Abstractions.ITestFrameworkDiscoveryOptions;

class FactDiscoverer :
    Xunit.Sdk.FactDiscoverer
{
    IMessageSink sink;

    public FactDiscoverer(IMessageSink sink)
        : base(sink)
    {
        this.sink = sink;
    }

    //public override IEnumerable<IXunitTestCase> Discover(Options options, ITestMethod method, IAttributeInfo attribute)
    //{
    //    return base.Discover(options, method, attribute)
    //        .Select(x => new TestCase((XunitTestCase) x, false));
    //}

    protected override IXunitTestCase CreateTestCase(Options options, ITestMethod method, IAttributeInfo attribute)
    {
        var xunitTestCase = (XunitTestCase) base.CreateTestCase(options, method, attribute);
        return new TestCase(xunitTestCase, false);
    }
}
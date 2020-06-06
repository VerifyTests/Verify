using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;
using Options=Xunit.Abstractions.ITestFrameworkDiscoveryOptions;

class FactDiscoverer :
    Xunit.Sdk.FactDiscoverer
{
    IMessageSink messageSink;

    public FactDiscoverer(IMessageSink messageSink)
        : base(messageSink)
    {
        this.messageSink = messageSink;
    }

    public override IEnumerable<IXunitTestCase> Discover(Options options, ITestMethod method, IAttributeInfo attribute)
    {
        return base.Discover(options, method, attribute)
            .Select(x => new TestCase(x));
    }

    protected override IXunitTestCase CreateTestCase(Options options, ITestMethod method, IAttributeInfo attribute)
    {
        return new TestCase(base.CreateTestCase(options, method, attribute));
    }
}
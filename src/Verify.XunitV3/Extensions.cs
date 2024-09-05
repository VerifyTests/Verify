static class Extensions
{
    public static IXunitTestMethod GetTestMethod(this TestContext context)
    {
        var method = context.TestMethod;
        if (method == null)
        {
            throw new("TestContext.TestMethod is null");
        }

        return (IXunitTestMethod) method;
    }
}
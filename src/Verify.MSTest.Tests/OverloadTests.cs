// Overloads share a name, so the data the test was invoked with is all there is
// to tell the MethodInfo of one from the other
[TestClass]
public partial class OverloadTests
{
    [TestMethod]
    [DataRow("Value")]
    public Task Overload(string value) =>
        Verify(value);

    [TestMethod]
    [DataRow(1, 2)]
    public Task Overload(int first, int second) =>
        Verify($"{first} {second}");

    // same parameter count, so only the types of the data tell these apart
    [TestMethod]
    [DataRow("Value")]
    public Task SameArityOverload(string text) =>
        Verify(text);

    [TestMethod]
    [DataRow(1)]
    public Task SameArityOverload(int number) =>
        Verify(number);
}

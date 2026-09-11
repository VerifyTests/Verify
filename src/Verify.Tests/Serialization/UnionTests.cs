// C# unions and closed type hierarchies are language features rather than runtime features, so
// they compile on every target framework here. Verify references Polyfill privately and grants
// Verify.Tests InternalsVisibleTo, so the marker attributes are in scope below net11.0 without any
// project change. That also makes these tests meaningful downlevel: the marker Verify.Tests
// compiles against is Verify's own internal copy, a different Type from the one inside Argon, so a
// green run proves Argon matches the marker by name rather than by a compile time type reference.

public class UnionTests
{
    // a union is written as its active case, with no wrapper and no discriminator

    [Fact]
    public Task NumberCase() =>
        Verify(new IntOrString(42));

    [Fact]
    public Task StringCase() =>
        Verify(new IntOrString("hi"));

    [Fact]
    public Task ObjectCase() =>
        Verify(new Pet(new Cat("Tibbles", true)));

    [Fact]
    public Task ListCase() =>
        Verify(new IntOrList(["a", "b"]));

    [Fact]
    public Task UnionAsMember() =>
        Verify(
            new Holder
            {
                Value = new(42),
                Name = "the name"
            });

    // a union holding no value writes null, which DefaultValueHandling.Ignore then drops
    [Fact]
    public Task DefaultUnionIsOmitted() =>
        Verify(new Holder {Name = "the name"});

    // scrubbing happens in VerifyJsonWriter, and the union converter writes the case value through
    // that same writer, so counters and scrubbers still apply inside a union
    [Fact]
    public Task ScrubbersApplyInsideUnion() =>
        Verify(new GuidOrString(Guid.Parse("11111111-1111-1111-1111-111111111111")));

    // both cases carry the same properties, so the snapshot cannot tell them apart. Verify only
    // writes, so this is a readability limitation rather than a round trip failure
    [Fact]
    public Task CasesWithMatchingShapeAreIndistinguishable() =>
        Verify(
            new
            {
                succeeded = new Outcome(new Succeeded("done")),
                failed = new Outcome(new Failed("done"))
            });

    // a closed hierarchy is serialized by runtime type with no discriminator, the same as any
    // other abstract base. Argon only infers a $type when InferClosedTypePolymorphism is enabled,
    // which Verify does not do
    [Fact]
    public Task ClosedTypeThroughProperty() =>
        Verify(new EventHolder {Event = new PaymentAuthorized("p-123", 42.5m)});

    [Fact]
    public Task ClosedTypeInList() =>
        Verify(
            new List<PaymentEvent>
            {
                new PaymentAuthorized("p-1", 1m),
                new PaymentCaptured("p-2", "r-2")
            });

    public class Holder
    {
        public IntOrString Value { get; set; }
        public string? Name { get; set; }
    }

    public class EventHolder
    {
        public PaymentEvent? Event { get; set; }
    }

    public union IntOrString(int, string);

    public union IntOrList(int, List<string>);

    public union GuidOrString(Guid, string);

    public union Pet(Cat, Dog);

    public union Outcome(Succeeded, Failed);

    public record Cat(string Name, bool Indoor);

    public record Dog(string Name, bool GoodBoy);

    public record Succeeded(string Message);

    public record Failed(string Message);

    public closed record PaymentEvent(string PaymentId);

    public sealed record PaymentAuthorized(string PaymentId, decimal Amount) : PaymentEvent(PaymentId);

    public sealed record PaymentCaptured(string PaymentId, string Reference) : PaymentEvent(PaymentId);
}

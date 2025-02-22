#region NumericIdSample

public class NumericIdSample
{
    public class Target : IHasId
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
    }

    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.ScrubMembers(
            _ => typeof(IHasId).IsAssignableFrom(_.DeclaringType) &&
                 _.Name == "Id");

    [Fact]
    public Task Test()
    {
        var target = new Target
        {
            Id = new Random().Next(),
            Name = "The Name"
        };
        return Verify(target);
    }

    public interface IHasId
    {
        public int Id { get; init; }
    }
}

#endregion
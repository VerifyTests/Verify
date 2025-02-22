#region NumericIdSample

using Polyfills;

public class NumericIdSample
{
    public class Target : IId
    {
        public int Id { get; init; }
        public string Name => $"Name {Id}";
    }

    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.ScrubMembers(Foo);
    }

    private static bool Foo(MemberInfo arg)
    {
        if (arg.DeclaringType.IsAssignableTo(typeof(IId)))
        {
            Counter.Current.NextString()
        }

        return false;
    }

    [Fact]
    public Task Lines()
    {
        var target = new Target()
        {
            Id = new Random().Next()
        };
        return Verify(target);
    }

    public interface IId
    {
        public int Id { get; init; }
    }
}

#endregion
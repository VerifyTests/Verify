[UsesVerify]
public class CircularTests
{
    public class Parent
    {
        public List<Child> Children { get; set; } = new();
    }

    public class Child
    {
        public Parent Parent { get; set; } = null!;
    }

    [Fact]
    public Task Simple()
    {
        var parent = new Parent();
        parent.Children.Add(new()
        {
            Parent = parent
        });
        parent.Children.Add(new()
        {
            Parent = parent
        });
        return Verify(parent);
    }
}
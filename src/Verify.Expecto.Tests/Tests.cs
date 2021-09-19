using Expecto;
using Expecto.CSharp;

public class Tests
{
    [Tests] 
    public static Test test1 = Runner.TestCase("standard",
        () =>
        {
            Console.Write("standard");
        });
}
public class Tests2
{
    [Tests] 
    public static Test test2 = Runner.TestCase("standard2",
        () =>
        {
            Expect.equal(1,2,"a");
        });
}
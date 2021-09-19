using Expecto;
using Expecto.CSharp;

public class Tests
{
    [Tests] 
    public static Test test = Runner.TestCase("standard",
        () =>
        {
            Console.Write("standard");
        });
}
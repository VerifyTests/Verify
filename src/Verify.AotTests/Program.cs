public class Program
{
    [Test]
    public Task Person()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
        // var person = new Person
        // {
        //     FirstName = "FirstName",
        //     LastName = "LastName"
        // };
        //
        // var settings = new VerifySettings();
        // await Verify(person, settings);
        // var responseBody = "{\"hello\":\"world\"}";
        // return Verify(responseBody);
    }
}

// public class Person
// {
//     public string FirstName { get; set; }
//     public string LastName { get; set; }
// }

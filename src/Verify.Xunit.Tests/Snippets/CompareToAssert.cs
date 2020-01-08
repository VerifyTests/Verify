using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class CompareToAssert :
    VerifyBase
{
    #region TraditionalTest
    [Fact]
    public void TraditionalTest()
    {
        var person = ClassBeingTested.FindPerson();
        Assert.Equal(new Guid("ebced679-45d3-4653-8791-3d969c4a986c"), person.Id);
        Assert.Equal(Title.Mr, person.Title);
        Assert.Equal("John", person.GivenNames);
        Assert.Equal("Smith", person.FamilyName);
        Assert.Equal("Jill", person.Spouse);
        Assert.Equal(2, person.Children.Count);
        Assert.Equal("Sam", person.Children[0]);
        Assert.Equal("Mary", person.Children[1]);
        Assert.Equal("4 Puddle Lane", person.Address.Street);
        Assert.Equal("USA", person.Address.Country);
    }
    #endregion

    #region VerificationTest
    [Fact]
    public Task Simple()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }
    #endregion

    public CompareToAssert(ITestOutputHelper output) :
        base(output)
    {
    }
}
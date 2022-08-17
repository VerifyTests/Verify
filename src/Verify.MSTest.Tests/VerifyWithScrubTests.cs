using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Verify.MSTest.Tests;

[TestClass]
public class VerifyWithScrubTests : VerifyBase
{
    [TestMethod]
    public async Task ScrubLinesWithReplace()
    {
        //Assemble
        var rng = RandomNumberGenerator.Create();

        var hash = new byte[32];
        rng.GetBytes(hash);



        //Act
        var testData = new
        {
            TestString = "Test string",
            SHA1 = SHA1.Create().ComputeHash(hash)
        };

        //Assert

        var settings = new VerifySettings();

        settings.DontScrubGuids();
        settings.ScrubLinesWithReplace(l => l.Trim().StartsWith("SHA1") ? $"{l.Substring(l.IndexOf(':'))} <Scrubbed>" : l);


        await Verify(testData, settings);
    }

    [TestMethod]
    public async Task ScrubLines()
    {
        //Assemble
        var rng = RandomNumberGenerator.Create();

        var hash = new byte[32];
        rng.GetBytes(hash);



        //Act
        var testData = new
        {
            TestString = "Test string",
            SHA1 = SHA1.Create().ComputeHash(hash)
        };

        //Assert

        var settings = new VerifySettings();

        settings.DontScrubGuids();
        settings.ScrubLines(l => l.Trim().StartsWith("SHA1"));


        await Verify(testData, settings);
    }

    [TestMethod]
    public async Task ScrubLinesContaining()
    {
        //Assemble
        var rng = RandomNumberGenerator.Create();

        var hash = new byte[32];
        rng.GetBytes(hash);



        //Act
        var testData = new
        {
            TestString = "Test string",
            SHA1 = SHA1.Create().ComputeHash(hash)
        };

        //Assert

        var settings = new VerifySettings();

        settings.DontScrubGuids();
        settings.ScrubLinesContaining("SHA1");


        await Verify(testData, settings);
    }
}

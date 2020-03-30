using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffEngine;
using EmptyFiles;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

public class Tests :
    VerifyBase
{
    [Fact]
    public Task Simple()
    {
        return Verify("Foo");
    }

    public Tests(ITestOutputHelper output) :
        base(output)
    {
    }
}
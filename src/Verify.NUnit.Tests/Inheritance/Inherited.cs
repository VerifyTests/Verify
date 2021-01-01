﻿using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;

public class Inherited : Base
{
    [Test]
    public override Task TestToOverride()
    {
        Trace.WriteLine("");
        return base.TestToOverride();
    }
}
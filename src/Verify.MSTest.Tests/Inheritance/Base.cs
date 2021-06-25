﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;

namespace TheTests
{
    [TestClass]
    public class Base :
        VerifyBase
    {
        [TestMethod]
        public Task TestInBase()
        {
            return Verify("Foo");
        }

        [TestMethod]
        public virtual Task TestToOverride()
        {
            return Verify("Foo");
        }
    }
}
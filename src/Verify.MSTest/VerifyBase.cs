﻿using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Verify;

namespace VerifyMSTest
{
    public partial class VerifyBase
    {
        public TestContext TestContext
        {
            get => testContext;
            set { testContext = value; }
        }

        TestContext testContext = null!;

        InnerVerifier BuildVerifier(string sourceFile, VerifySettings? settings)
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            var type = GetType();

            var methodInfo = type.GetMethod(testContext.TestName, BindingFlags.Instance | BindingFlags.Public);

            var uniqueTestName = TestNameBuilder.GetUniqueTestName(
                className: Path.GetFileNameWithoutExtension(sourceFile),
                methodInfo,
                settings.GetParameters(methodInfo));
            return new InnerVerifier(uniqueTestName, sourceFile);
        }
    }
}
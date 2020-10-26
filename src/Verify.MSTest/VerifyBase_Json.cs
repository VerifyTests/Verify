﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyMSTest
{
    public partial class VerifyBase
    {
        public async Task Verify<T>(
            Task<T> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings ??= new VerifySettings();
            using var verifier = BuildVerifier(settings, sourceFile);
            await verifier.Verify(target);
        }

        public async Task Verify<T>(
            ValueTask<T> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings ??= new VerifySettings();
            using var verifier = BuildVerifier(settings, sourceFile);
            await verifier.Verify(target);
        }

        public async Task Verify<T>(
            IAsyncEnumerable<T> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings ??= new VerifySettings();
            using var verifier = BuildVerifier(settings, sourceFile);
            await verifier.Verify(target);
        }

        public async Task Verify<T>(
            T target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            settings ??= new VerifySettings();
            using var verifier = BuildVerifier(settings, sourceFile);
            await verifier.Verify(target);
        }
    }
}
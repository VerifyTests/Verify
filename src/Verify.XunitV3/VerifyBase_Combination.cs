﻿// ReSharper disable InconsistentNaming
namespace VerifyXunit;

public partial class VerifyBase
{
    [Pure]
    public Combination Combination(
        bool? captureExceptions = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verifier.Combination(captureExceptions, settings ?? this.settings, sourceFile);
}
﻿namespace VerifyNUnit;

public static partial class Verifier
{

#if NETSTANDARD2_1 || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER

    public static SettingsTask VerifyDirectory(
        string path,
        Func<string, bool>? include = null,
        string? pattern = null,
        EnumerationOptions? options = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyDirectory(path, include, pattern, options), true);

    public static SettingsTask VerifyDirectory(
        DirectoryInfo path,
        Func<string, bool>? include = null,
        string? pattern = null,
        EnumerationOptions? options = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        VerifyDirectory(path.FullName, include, pattern, options, settings, sourceFile);

#else

    public static SettingsTask VerifyDirectory(
        string path,
        Func<string, bool>? include = null,
        string? pattern = null,
        SearchOption option = SearchOption.AllDirectories,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyDirectory(path, include, pattern, option), true);

    public static SettingsTask VerifyDirectory(
        DirectoryInfo path,
        Func<string, bool>? include = null,
        string? pattern = null,
        SearchOption option = SearchOption.AllDirectories,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        VerifyDirectory(path.FullName, include, pattern, option, settings, sourceFile);

#endif

}
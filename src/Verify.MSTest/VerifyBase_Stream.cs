﻿namespace VerifyMSTest;

public partial class VerifyBase
{
    public SettingsTask Verify(
        byte[]? target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, extension));

    public SettingsTask Verify(
        Task<byte[]> target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, extension));

    public SettingsTask Verify(
        FileStream? target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target));

    public SettingsTask Verify(
        Task<FileStream> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target));

    public SettingsTask Verify(
        Stream? target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, extension));

    public SettingsTask Verify<T>(
        Task<T> target,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
        where T : Stream =>
        Verify(settings, sourceFile, _ => _.VerifyStream(target, extension));

    public SettingsTask Verify<T>(
        IEnumerable<T> targets,
        string extension,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
        where T : Stream =>
        Verify(settings, sourceFile, _ => _.VerifyStreams(targets, extension));
}
// ReSharper disable InconsistentNaming
namespace VerifyTests;

public partial class Combination(
    bool? captureExceptions,
    VerifySettings? settings,
    string sourceFile,
    Func<VerifySettings?, string, Func<InnerVerifier, Task<VerifyResult>>, SettingsTask> verify);
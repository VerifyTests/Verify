namespace VerifyTests;

public partial class Combination(
    bool? captureExceptions,
    VerifySettings? settings,
    bool? header,
    string sourceFile,
    Func<VerifySettings?, string, Func<InnerVerifier, Task<VerifyResult>>, SettingsTask> verify);
namespace VerifyTests;

public partial class Combination(
    bool? captureExceptions,
    VerifySettings? settings,
    bool? header,
    string sourceFile,
    int lineNumber,
    Func<VerifySettings?, string, int, Func<InnerVerifier, Task<VerifyResult>>, SettingsTask> verify);
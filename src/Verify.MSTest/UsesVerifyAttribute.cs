namespace VerifyMSTest;

/// <summary>
/// No longer required, and no longer read by anything.
/// <para>
/// This marked a test class for the source generator, which injected a <c>TestContext</c> property
/// to capture the running test. That is now taken from <c>TestContext.Current</c>, so no opt in is
/// needed. Retained so existing code continues to compile.
/// </para>
/// </summary>
[Obsolete("UsesVerifyAttribute is no longer required and has no effect. Remove it.")]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class)]
public sealed class UsesVerifyAttribute : Attribute;

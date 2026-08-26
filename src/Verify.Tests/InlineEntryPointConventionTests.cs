/// <summary>
/// The names an accept may chain a <c>Snapshot</c> call onto live in DiffEngine, and the entry
/// points they stand for live here. Nothing connects the two but this test.
/// <para>
/// A name that goes missing fails silently and in the worst way available: the call site check
/// finds nothing to append to, the verification quietly keeps using a file, and the only symptom
/// is that a whole family of tests stopped inlining. Asked through the same public API the check
/// uses, rather than against a list of names, so what is proven is what actually happens.
/// </para>
/// </summary>
public class InlineEntryPointConventionTests
{
    /// <summary>
    /// Every entry point, through every receiver one is reached by. The receiver rule is what
    /// keeps an accept off a project's own <c>ContentValidation.Verify(...)</c>, and it is applied
    /// per call rather than per name, so a name that only ever appears unqualified in a fixture
    /// leaves the pairing untested.
    /// </summary>
    [Fact]
    public void EveryEntryPointCanBeAppendedTo()
    {
        var names = EntryPointNames().ToList();
        // A filter that stopped matching would pass this test by having nothing to check
        Assert.NotEmpty(names);

        foreach (var name in names)
        {
            // Unqualified from a static using, on the Verifier class, and inherited from VerifyBase
            AssertCanAppendTo($"{name}(value)");
            AssertCanAppendTo($"Verifier.{name}(value)");
            AssertCanAppendTo($"this.{name}(value)");
        }
    }

    /// <summary>
    /// The other half of the same rule: a member of the project's own, reached through a receiver
    /// of its own, is not an entry point however it is named.
    /// </summary>
    [Fact]
    public void AForeignReceiverIsNeverAnEntryPoint()
    {
        foreach (var name in EntryPointNames())
        {
            AssertCannotAppendTo($"ContentValidation.{name}(value)");
        }
    }

    static void AssertCanAppendTo(string call) =>
        Assert.True(
            CanAppendTo(call) == InlineApplyStatus.Applied,
            $"`{call}` is an entry point call that an inline snapshot cannot be accepted into. Add the name to DiffEngine's built-in entry points.");

    static void AssertCannotAppendTo(string call) =>
        Assert.True(
            CanAppendTo(call) == InlineApplyStatus.NotFound,
            $"`{call}` is not an entry point, and accepting an inline snapshot into it would write source that does not compile.");

    static InlineApplyStatus CanAppendTo(string call)
    {
        var file = Path.Combine(Path.GetTempPath(), $"InlineEntryPointConventionTests_{Guid.NewGuid():N}.cs");
        File.WriteAllText(file, $"class Tests\n{{\n    Task Test() => {call};\n}}\n");
        try
        {
            var patch = new InlinePatch(file, 3, null, "content", InlinePatchMode.Append)
            {
                TestName = null,
                MemberName = "Test"
            };

            return InlineApplier.CanAnchor(patch).Status;
        }
        finally
        {
            File.Delete(file);
        }
    }

    /// <summary>
    /// Everything a test can write that ends in a verification: the ones that return a
    /// <see cref="SettingsTask" />, and <c>Combination</c>, which returns the builder for one and
    /// is the call the caller info was captured at.
    /// </summary>
    static IEnumerable<string> EntryPointNames() =>
        typeof(Verifier)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(_ => _.ReturnType == typeof(SettingsTask) ||
                        _.ReturnType == typeof(Combination))
            .Select(_ => _.Name)
            .Distinct();
}

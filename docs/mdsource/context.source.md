# Context

`VerifySettings.Context` is a dictionary that carries information from a test to the extension points that run during a verification. It is exposed as `IReadOnlyDictionary<string, object>` to:

 * [Comparers](/docs/comparer.md)
 * [Converters](/docs/converter.md)
 * [Scrubbers](/docs/scrubbers.md)
 * [AppendFile](/docs/append-file.md) and [JsonAppender](/docs/jsonappender.md) delegates

It is also exposed as `VerifyJsonWriter.Context` to serialization converters.

Those extension points are registered globally, and are shared by every test. Context is how a single test passes state to them, for example an environment name or a feature flag.

Values are written to the dictionary on the settings used for the verification:

snippet: ContextInTest

Or fluently, via `AddContext`:

snippet: ContextInTestFluent

And read in the extension point:

snippet: ContextInComparer

Values that are the same for every test do not need Context. A static field is sufficient in that case. Context matters where the value varies per test.


## Reserved keys

Verify uses the same dictionary for some per-verification state, under keys prefixed with `Verify.`. For example `ExcludeTargets` stores its extensions under `Verify.ExcludeTargets`, which is what allows a converter to call `context.IsTargetExcluded("png")`. Keys prefixed with `Verify.` should be treated as reserved.


## Copy behavior

When settings are copied, for example when a `VerifySettings` instance is passed to a fluent API, the context is copied entry by entry. Entries implementing `ICloneable` are cloned, and all other entries are copied by reference.

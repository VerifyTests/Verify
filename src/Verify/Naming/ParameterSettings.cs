namespace VerifyTests;

public partial class VerifySettings
{
    object?[]? parameters;

    public bool TryGetParameters([NotNullWhen(true)] out object?[]? parameters)
    {
        if (this.parameters == null)
        {
            parameters = null;
            return false;
        }

        parameters = this.parameters;
        return true;
    }

    public bool HasParameters => parameters != null;

    /// <inheritdoc cref="UseParameters(object?[])"/>
    public void UseParameters<T>(T[] parameters) =>
        UseParameters(
            new object?[]
            {
                parameters
            });

    /// <inheritdoc cref="UseParameters(object?[])"/>
    public void UseParameters<T>(T parameter) =>
        UseParameters(
            new object?[]
            {
                parameter
            });

    /// <summary>
    /// Define the parameter values being used by a parameterised (aka data driven) test.
    ///
    /// Scenarios:
    ///
    /// <list type="bullet">
    ///   <item>Verify.Expecto: Does not currently support `UseParameters()`.</item>
    ///   <item>Verify.Fixie: Automatically detects the method parameters via a custom ITestProject https://github.com/VerifyTests/Verify/blob/main/docs/parameterised.md#fixie.</item>
    ///   <item>Verify.MSTest: Does not detect the parametrised arguments, as such `UseParameters()` is required.</item>
    ///   <item>Verify.NUnit: Automatically detects the method parameters. So `UseParameters()` is not required unless using custom parameters.</item>
    ///   <item>Verify.Xunit: Does not detect the parametrised arguments, as such `UseParameters()` is required.</item>
    ///   <item>Verify.XunitV3: Automatically detect the method parameters for built in types (string, int, bool etc), but for complex types `UseParameters()` is required.</item>
    /// </list>
    ///
    /// In the scenarios where parameters are not automatically detected, an exception will be thrown instructing the potential need for <see cref="UseParameters" />
    /// Not compatible with <see cref="UseTextForParameters" />.
    /// </summary>
    public void UseParameters(params object?[] parameters)
    {
        Guard.NotNullOrEmpty(parameters);
        ThrowIfFileNameDefined();
        ThrowIfParametersTextDefined();
        this.parameters = parameters;
    }

    [Experimental("VerifySetParameters")]
    public void SetParameters(object?[] parameters) =>
        this.parameters = parameters;

    void ThrowIfParametersTextDefined([CallerMemberName] string caller = "")
    {
        if (parametersText is not null)
        {
            throw new($"{caller} is not compatible with {nameof(UseTextForParameters)}.");
        }
    }

    internal bool ignoreParametersForVerified;

    /// <summary>
    /// Ignore parameters in 'verified' filename resulting in the same verified file for each testcase.
    /// Note that the 'received' files contain the parameters.
    /// </summary>
    public void IgnoreParametersForVerified(params object?[] parameters)
    {
        if (parameters.Length > 0)
        {
            UseParameters(parameters);
        }

        ignoreParametersForVerified = true;
    }

    internal bool hashParameters;

    /// <summary>
    /// Hash parameters together and pass to <see cref="UseTextForParameters" />.
    /// Used to get a deterministic file name while avoiding long paths.
    /// </summary>
    public void HashParameters()
    {
        ThrowIfFileNameDefined();
        ThrowIfParametersTextDefined();
        hashParameters = true;
    }

    /// <summary>
    /// Provide parameters to hash together and pass to <see cref="UseTextForParameters" />.
    /// Used to get a deterministic file name while avoiding long paths.
    /// Combines <see cref="UseParameters" /> and <see cref="HashParameters" />.
    /// </summary>
    public void UseHashedParameters(params object?[] parameters)
    {
        UseParameters(parameters);
        HashParameters();
    }
}
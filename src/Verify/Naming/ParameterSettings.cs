namespace VerifyTests;

public partial class VerifySettings
{
    internal object?[]? parameters;

    /// <summary>
    /// Define the parameter values being used by a parameterised (aka data driven) test.
    /// In most cases the parameter values can be automatically resolved.
    /// When this is not possible, an exception will be thrown instructing the use of <see cref="UseParameters" />
    /// Not compatible with <see cref="UseTextForParameters" />.
    /// </summary>
    public void UseParameters<T>(T[] parameters) =>
        UseParameters(
            new object?[]
            {
                parameters
            });

    public void UseParameters<T>(T parameter) =>
        UseParameters(
            new object?[]
            {
                parameter
            });

    public void UseParameters(params object?[] parameters)
    {
        Guard.AgainstNullOrEmpty(parameters);
        ThrowIfFileNameDefined();
        ThrowIfParametersTextDefined();
        this.parameters = parameters;
    }

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
        UseParameters(parameters);
        ignoreParametersForVerified = true;
    }

#if NET6_0_OR_GREATER || NETFRAMEWORK
    internal bool hashParameters;

    /// <summary>
    /// Hash parameters together and pass to <see cref="UseTextForParameters"/>.
    /// Used to get a deterministic file name while avoiding long paths.
    /// </summary>
    public void HashParameters()
    {
        ThrowIfFileNameDefined();
        ThrowIfParametersTextDefined();
        hashParameters = true;
    }

    /// <summary>
    /// Provide parameters to hash together and pass to <see cref="UseTextForParameters"/>.
    /// Used to get a deterministic file name while avoiding long paths.
    /// Combines <see cref="UseParameters"/> and <see cref="HashParameters"/>.
    /// </summary>
    public void UseHashedParameters(params object?[] parameters)
    {
        UseParameters(parameters);
        HashParameters();
    }
#endif
}
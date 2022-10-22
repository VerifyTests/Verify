namespace VerifyTests;

public partial class VerifySettings
{
    IReadOnlyCollection<object?>? IVerifySettings.Parameters => parameters;

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
        Guard.AgainstNullOrEmpty(parameters, nameof(parameters));
        ThrowIfFileNameDefined();
        if (parametersText is not null)
        {
            throw new($"{nameof(UseParameters)} is not compatible with {nameof(UseTextForParameters)}.");
        }

        this.parameters = parameters;
    }

    bool IVerifySettings.IgnoreParametersForVerified => ignoreParametersForVerified;
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

}
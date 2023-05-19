using System.Security.Cryptography;

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

    public void UseParametersHash(params object?[] parameters)
    {
        Guard.AgainstNullOrEmpty(parameters);
        ThrowIfFileNameDefined();
        ThrowIfParametersTextDefined();

        StringBuilder paramsToHash = new();

        foreach (object? value in parameters)
        {
            string? s = value switch
            {
                null => "null",
                string[] a => string.Join(",", a),
                IEnumerable<object> e => string.Join(",", e.Select(x => x.ToString())),
                _ => value.ToString()
            };

            paramsToHash.Append(s);
        }

        using SHA256 hasher = SHA256.Create();
        byte[] data = hasher.ComputeHash(Encoding.UTF8.GetBytes(paramsToHash.ToString()));

        StringBuilder hashBuilder = new();

        for (int i = 0; i < data.Length; i++)
        {
            hashBuilder.Append(data[i].ToString("x2"));
        }

        UseTextForParameters(hashBuilder.ToString());
    }
}
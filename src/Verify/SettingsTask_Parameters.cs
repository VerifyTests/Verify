namespace VerifyTests;

public partial class SettingsTask
{
    /// <summary>
    /// Ignore parameters in 'verified' filename resulting in the same verified file for multiple testcases.
    /// Note that UseParameters has still been called for test frameworks that don't support automatic parameter detection and the 'received' files still contains the parameters.
    /// </summary>
    /// <param name="parameterNames">The names of the parameters to be ignored. When passing an empty list all parameters will be ignored.</param>
    [Pure]
    public SettingsTask IgnoreParameters(params string[] parameterNames)
    {
        CurrentSettings.IgnoreParameters(parameterNames);
        return this;
    }

    /// <summary>
    /// Ignore all parameters in 'verified' filename resulting in the same verified file for each testcase.
    /// Note that the 'received' files still contain the parameters.
    /// </summary>
    /// <param name="parameters">The parameters as you would have passed them to the UseParameters function.</param>
    [Pure]
    public SettingsTask IgnoreParametersForVerified(params object?[] parameters)
    {
        CurrentSettings.IgnoreParametersForVerified(parameters);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UseParameters(object?[])"/>
    [Pure]
    public SettingsTask UseParameters(params object?[] parameters)
    {
        CurrentSettings.UseParameters(parameters);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UseParameters(object?[])"/>
    [Pure]
    public SettingsTask UseParameters<T>(T parameter)
    {
        CurrentSettings.UseParameters(parameter);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.UseParameters(object?[])"/>
    [Pure]
    public SettingsTask UseParameters<T>(T[] parameters)
    {
        CurrentSettings.UseParameters(parameters);
        return this;
    }

    /// <summary>
    /// Hash parameters together and pass to <see cref="UseTextForParameters" />.
    /// Used to get a deterministic file name while avoiding long paths.
    /// </summary>
    [Pure]
    public SettingsTask HashParameters()
    {
        CurrentSettings.HashParameters();
        return this;
    }


    /// <summary>
    /// Use a custom text for the `Parameters` part of the file name.
    /// Not compatible with <see cref="UseParameters" />.
    /// Where the file format is `{CurrentDirectory}/{TestClassName}.{TestMethodName}_{Parameters}_{UniqueFor1}_{UniqueFor2}_{UniqueForX}.verified.{extension}`.
    /// </summary>
    [Pure]
    public SettingsTask UseTextForParameters(string parametersText)
    {
        CurrentSettings.UseTextForParameters(parametersText);
        return this;
    }
}
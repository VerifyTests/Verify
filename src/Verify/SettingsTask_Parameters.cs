namespace VerifyTests;

public partial class SettingsTask
{
    /// <inheritdoc cref="VerifySettings.IgnoreParameters(string[])"/>
    [Pure]
    public SettingsTask IgnoreParameters(params string[] parameterNames)
    {
        CurrentSettings.IgnoreParameters(parameterNames);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.IgnoreParametersForVerified(object?[])"/>
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

    /// <inheritdoc cref="VerifySettings.UseTextForParameters(string)"/>
    [Pure]
    public SettingsTask UseTextForParameters(string parametersText)
    {
        CurrentSettings.UseTextForParameters(parametersText);
        return this;
    }
}
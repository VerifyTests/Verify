namespace VerifyTests
{
    public partial class VerifySettings
    {
        internal object?[]? parameters;

        /// <summary>
        /// Define the parameter values being used by a parameterised (aka data drive) test.
        /// In most cases the parameter values can be automatically resolved.
        /// When this is not possible, an exception will be thrown instructing the use of <see cref="UseParameters"/>
        /// Not compatible with <see cref="UseTextForParameters"/>.
        /// </summary>
        public void UseParameters(params object?[] parameters)
        {
            Guard.AgainstNullOrEmpty(parameters, nameof(parameters));
            ThrowIfFileNameDefined();
            if (parametersText != null)
            {
                throw new($"{nameof(UseParameters)} is not compatible with {nameof(UseTextForParameters)}.");
            }

            this.parameters = parameters;
        }
    }
}
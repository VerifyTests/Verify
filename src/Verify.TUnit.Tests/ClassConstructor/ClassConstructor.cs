public class ClassConstructor :
    IClassConstructor
{
    static readonly ServiceProvider provider = CreateServiceProvider();

    AsyncServiceScope scope;

    static ServiceProvider CreateServiceProvider() =>
        new ServiceCollection()
            .AddSingleton<string>("SingletonValue")
            .BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });

    public Task<object> Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type, ClassConstructorMetadata metadata)
    {
        scope = provider.CreateAsyncScope();
        return Task.FromResult(ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, type));
    }
}
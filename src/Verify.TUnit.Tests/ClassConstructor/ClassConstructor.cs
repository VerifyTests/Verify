public class ClassConstructor :
    IClassConstructor
{
    static readonly ServiceProvider serviceProvider = CreateServiceProvider();

    AsyncServiceScope scope;

    static ServiceProvider CreateServiceProvider() =>
        new ServiceCollection()
            .AddSingleton<string>("SingletonValue")
            .BuildServiceProvider();

    public object Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type, ClassConstructorMetadata classConstructorMetadata)
    {
        scope = serviceProvider.CreateAsyncScope();
        return ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, type);
    }
}
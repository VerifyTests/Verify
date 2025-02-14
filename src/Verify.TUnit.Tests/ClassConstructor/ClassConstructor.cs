using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Interfaces;

public class ClassConstructor : IClassConstructor
{
    static readonly ServiceProvider serviceProvider = CreateServiceProvider();

    AsyncServiceScope scope;

    public T Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(ClassConstructorMetadata classConstructorMetadata)
        where T : class
    {
        scope = serviceProvider.CreateAsyncScope();
        return ActivatorUtilities.GetServiceOrCreateInstance<T>(scope.ServiceProvider);
    }

    static ServiceProvider CreateServiceProvider() =>
        new ServiceCollection()
            .AddSingleton<string>("SingletonValue")
            .BuildServiceProvider();
}
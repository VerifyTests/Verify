using Microsoft.Extensions.DependencyInjection;
using TUnit.Core.Interfaces;

public class DependencyInjectionClassConstructor : IClassConstructor, ITestEndEvent
{
    private static readonly IServiceProvider _serviceProvider = CreateServiceProvider();

    private AsyncServiceScope _scope;

    public T Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(ClassConstructorMetadata classConstructorMetadata)
        where T : class
    {
        _scope = _serviceProvider.CreateAsyncScope();

        return ActivatorUtilities.GetServiceOrCreateInstance<T>(_scope.ServiceProvider);
    }

    public ValueTask OnTestEnd(TestContext testContext)
    {
        return _scope.DisposeAsync();
    }

    private static IServiceProvider CreateServiceProvider()
    {
        return new ServiceCollection()
            .AddSingleton<SomeClass1>()
            .AddSingleton<SomeClass2>()
            .AddTransient<SomeClass3>()
            .BuildServiceProvider();
    }
}

[ClassConstructor<DependencyInjectionClassConstructor>]
public class MyTestClass(SomeClass1 someClass1, SomeClass2 someClass2, SomeClass3 someClass3)
{
    [Test]
    public async Task Test()
    {
        // ...
    }
}
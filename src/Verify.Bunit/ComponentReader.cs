using System.Linq;
using Bunit;

static class ComponentReader
{
    public static object? GetInstance(IRenderedFragment fragment)
    {
        var type = fragment.GetType();
        if (!type.IsGenericType)
        {
            return null;
        }

        var renderComponentInterface = type
            .GetInterfaces()
            .SingleOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IRenderedComponentBase<>));
        if (renderComponentInterface == null)
        {
            return null;
        }

        var instanceProperty = renderComponentInterface.GetProperty("Instance");
        return instanceProperty.GetValue(fragment);
    }
}
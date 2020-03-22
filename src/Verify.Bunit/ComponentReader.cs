using System.Linq;
using Bunit;

class ComponentReader
{
    public static object? GetInstance(IRenderedFragment fragment)
    {
        var type = fragment.GetType();
        if (!type.IsGenericType)
        {
            return null;
        }

        var renderComponentInterface = type.GetInterfaces()
            .Where(x => x.IsGenericType &&
                        x.GetGenericTypeDefinition() == typeof(IRenderedComponent<>))
            .SingleOrDefault();
        if (renderComponentInterface  == null)
        {
            return null;
        }

        var instanceProperty = renderComponentInterface.GetProperty("Instance");
        return instanceProperty.GetValue(fragment);
    }
}
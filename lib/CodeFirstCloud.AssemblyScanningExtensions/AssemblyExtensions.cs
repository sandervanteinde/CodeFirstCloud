using System.Reflection;

namespace CodeFirstCloud.AssemblyScanningExtensions;

public static class AssemblyExtensions
{
    public static IEnumerable<(Type, TAttribute)> ScanForHandlerWithAttribute<THandler, TAttribute>(this Assembly assembly)
        where TAttribute : Attribute
    {
        var handlerType = typeof(THandler);

        foreach (var item in assembly.GetTypes())
        {
            if (!item.IsAssignableTo(handlerType))
            {
                continue;
            }

            var attr = item.GetCustomAttribute<TAttribute>();

            if (attr is null)
            {
                throw new InvalidOperationException($"Type {item.Name} is extending {handlerType} but is not decorated with {typeof(TAttribute)}.");
            }

            yield return (item, attr);
        }
    }
}
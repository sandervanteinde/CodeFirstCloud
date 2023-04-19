using System.Reflection;

namespace CodeFirstCloud.AssemblyScanningExtensions;

public static class AssemblyExtensions
{
    public static IEnumerable<(Type, TAttribute)> ScanForClassesWithAttribute<TClass, TAttribute>(this Assembly assembly)
        where TAttribute : Attribute
    {
        var classType = typeof(TClass);

        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsAssignableTo(classType))
            {
                continue;
            }

            var attribute = type.GetCustomAttribute<TAttribute>();

            if (attribute is null)
            {
                throw new InvalidOperationException($"Type {type.Name} is extending {classType} but is not decorated with {typeof(TAttribute)}.");
            }

            yield return (type, attribute);
        }
    }
}
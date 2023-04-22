using System.Diagnostics.CodeAnalysis;
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

public static class TypeExtensions
{
    public static bool IsAssignableToGenericType(this Type givenType, Type genericType, [NotNullWhen(true)] out Type? @interface)
    {
        var typeToCheck = givenType;

        while (typeToCheck is not null)
        {
            @interface = null;
            var interfaceTypes = typeToCheck.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    @interface = it;
                    return true;
                }
            }

            if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == genericType)
            {
                @interface = typeToCheck;
                return true;
            }

            typeToCheck = typeToCheck.BaseType;
        }

        @interface = null;
        return false;
    }
}
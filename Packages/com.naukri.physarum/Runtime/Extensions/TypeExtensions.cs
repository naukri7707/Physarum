using System;
using System.Linq;

namespace Naukri.Physarum.Extensions
{
    public static class TypeExtensions
    {
        public static string GetFriendlyGenericName(this Type type)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            if (!type.IsGenericType)
            {
                return type.Name;
            }

            var genericArguments = type.GetGenericArguments();
            var typeDefinition = type.GetGenericTypeDefinition().Name;
            typeDefinition = typeDefinition[..typeDefinition.IndexOf('`')];

            return $"{typeDefinition}<{string.Join(", ", genericArguments.Select(t => GetFriendlyGenericName(t)))}>";
        }
    }
}

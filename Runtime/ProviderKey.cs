using System;

namespace Naukri.Physarum
{
    public record ProviderKey
    {
        private static int uniqueId = 0;

        public static ProviderKey Unique()
        {
            return new ProviderKey<int>(uniqueId++);
        }

        public static ProviderKey FromType(Type type)
        {
            return new ProviderKey<Type>(type);
        }
    }

    public record ProviderKey<T>(T Key) : ProviderKey;
}

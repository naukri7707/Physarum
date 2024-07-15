using System;
using UnityEngine.Assertions;

namespace Naukri.Physarum.Core
{
    internal static class ProviderLocator
    {
        public static T Get<T>()
            where T : IProvider
        {
            var key = ProviderKey.FromType(typeof(T));

            return Get<T>(key);
        }

        public static T Get<T>(ProviderKey key)
            where T : IProvider
        {
            var container = ProviderContainer.LocateOrCreate();
            var provider = container.GetProvider(key);

            Assert.IsNotNull(provider, $"Can not found provider {typeof(T).Name} with key {key}.");

            if (provider is not T typedProvider)
            {
                throw new InvalidCastException(
                    $"Provider of type {provider.GetType().Name} cannot be cast to {typeof(T).Name}."
                );
            }

            return typedProvider;
        }
    }
}

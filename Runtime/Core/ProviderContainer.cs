using System;
using System.Collections.Generic;
using Naukri.Physarum.Utils;
using UnityEngine;

namespace Naukri.Physarum.Core
{
    public class ResolveEventArgs : EventArgs
    {
        private readonly HashSet<IProvider> providers = new();

        internal IEnumerable<IProvider> Providers => providers;

        public void AddProvider(IProvider provider)
        {
            providers.Add(provider);
        }
    }

    [AddComponentMenu("")] // Hide ProviderContainer in AddComponent's Menu
    public sealed partial class ProviderContainer : MonoBehaviour, IComponentCreatedHandler
    {
        public event EventHandler<ResolveEventArgs> Resolver = (_, e) =>
        {
            var providers = FindObjectsByType<Provider.Behaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

            foreach (var provider in providers)
            {
                e.AddProvider(provider);
            }
        };

        private readonly Dictionary<ProviderKey, IProvider> cachedProviders = new();

        #region methods

        internal IProvider GetProvider(ProviderKey key)
        {
            if (!cachedProviders.ContainsKey(key))
            {
                // If we can't find the provider in cache,
                // resolve all providers again.
                Resolve(key);
            }
            var provider = cachedProviders[key];

            return provider;
        }

        internal bool Register(IProvider provider)
        {
            if (provider == null)
            {
                return false;
            }

            var key = provider.Key;

            if (cachedProviders.TryGetValue(key, out var cachedProvider))
            {
                // Throw exception if provider's key has already been registered and is not the same object
                if (!ReferenceEquals(provider, cachedProvider))
                {
                    throw new ArgumentException(
                        $"Provider with key '{key}' has already been registered",
                        nameof(provider)
                    );
                }
                // Pass if provider's key has already been registered and is the same object
                else
                {
                    return false;
                }
            }
            else
            {
                // Register provider if key has not been registered
                cachedProviders[key] = provider;
                return true;
            }
        }

        internal bool Unregister(IProvider provider)
        {
            var key = provider.Key;

            return cachedProviders.Remove(key);
        }

        private void Resolve(object sender)
        {
            var e = new ResolveEventArgs();

            Resolver?.Invoke(sender, e);

            foreach (var provider in e.Providers)
            {
                Register(provider);
            }
        }

        private void OnDestroy()
        {
            ComponentLocator<ProviderContainer>.Invalidate();
        }

        void IComponentCreatedHandler.OnComponentCreated()
        {
            gameObject.name = kInstanceName;
        }

        #endregion
    }

    partial class ProviderContainer
    {
        internal const string kInstanceName = "[Provider Container]";

        internal static ProviderContainer LocateOrCreate()
        {
            var container = ComponentLocator<ProviderContainer>.FindOrCreateComponent(
                dontDestroyOnLoad: true
            );

            return container;
        }

        internal static bool TryLocate(out ProviderContainer container)
        {
            return ComponentLocator<ProviderContainer>.TryFindComponent(out container);
        }
    }
}

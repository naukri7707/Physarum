using System;

namespace Naukri.Physarum.Core
{
    public abstract class Context : IContext
    {
        private protected Context(Element element)
        {
            this.element = element;
        }

        internal readonly Element element;

        #region methods

        public TProvider Read<TProvider>()
            where TProvider : IProvider
        {
            var provider = ProviderLocator.Get<TProvider>();
            // Make sure the element is validated
            provider.EnsureValidated();

            return provider;
        }

        public TProvider Read<TProvider>(ProviderKey key)
            where TProvider : IProvider
        {
            var provider = ProviderLocator.Get<TProvider>(key);
            // Make sure the element is validated
            provider.EnsureValidated();

            return provider;
        }

        public TProvider Read<TProvider>(ProviderKeyOf<TProvider> key)
            where TProvider : IProvider
        {
            var provider = ProviderLocator.Get<TProvider>(key);
            // Make sure the element is validated
            provider.EnsureValidated();

            return provider;
        }

        public abstract TProvider Watch<TProvider>()
            where TProvider : IProvider;

        public abstract TProvider Watch<TProvider>(ProviderKey key)
            where TProvider : IProvider;

        public abstract TProvider Watch<TProvider>(ProviderKeyOf<TProvider> key)
            where TProvider : IProvider;

        public abstract Subscription Listen<TProvider>()
            where TProvider : IProvider;

        public abstract Subscription Listen<TProvider>(ProviderKey key)
            where TProvider : IProvider;

        public abstract Subscription Listen<TProvider>(out TProvider provider)
            where TProvider : IProvider;

        public abstract Subscription Listen<TProvider>(ProviderKey key, out TProvider provider)
            where TProvider : IProvider;

        public abstract Subscription Listen<TProvider>(
            ProviderKeyOf<TProvider> key,
            out TProvider provider
        )
            where TProvider : IProvider;

        public abstract Subscription Listen<TProvider>(TProvider provider)
            where TProvider : IProvider;

        public void Dispatch(IElementEvent evt)
        {
            Element.DispatchEvent(element, element, evt);
        }

        public abstract void DispatchListeners(IElementEvent evt);

        public void Refresh()
        {
            Dispatch(ElementEvents.Refresh.Default);
        }

        public void Invalidate()
        {
            Dispatch(ElementEvents.Invalidate.Default);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
        #endregion
    }
}

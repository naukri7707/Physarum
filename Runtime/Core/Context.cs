using System;

namespace Naukri.Physarum.Core
{
    public abstract class Context : IContext
    {
        private protected Context(IElement self)
        {
            this.self = self;
        }

        internal readonly IElement self;

        #region methods

        public TProvider Find<TProvider>()
            where TProvider : IProvider
        {
            return ProviderLocator.Get<TProvider>();
        }

        public TProvider Find<TProvider>(ProviderKey key)
            where TProvider : IProvider
        {
            return ProviderLocator.Get<TProvider>(key);
        }

        public abstract TProvider Watch<TProvider>()
            where TProvider : IProvider;

        public abstract TProvider Watch<TProvider>(ProviderKey key)
            where TProvider : IProvider;

        public abstract Subscription Listen<TProvider>()
            where TProvider : IProvider;

        public abstract Subscription Listen<TProvider>(ProviderKey key)
            where TProvider : IProvider;

        public abstract Subscription Listen<TProvider>(out TProvider provider)
            where TProvider : IProvider;

        public abstract Subscription Listen<TProvider>(ProviderKey key, out TProvider provider)
            where TProvider : IProvider;

        public abstract Subscription Listen<TProvider>(TProvider provider)
            where TProvider : IProvider;

        public void Dispatch(IElementEvent evt)
        {
            self.HandleEvent(self, evt);
        }

        public abstract void DispatchListeners(IElementEvent evt);

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
        #endregion
    }
}

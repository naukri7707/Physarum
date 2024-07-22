using System;

namespace Naukri.Physarum
{
    public interface IContext : IDisposable
    {
        TProvider Read<TProvider>()
            where TProvider : IProvider;
        TProvider Read<TProvider>(ProviderKey key)
            where TProvider : IProvider;
        TProvider Read<TProvider>(ProviderKeyOf<TProvider> key)
            where TProvider : IProvider;
        TProvider Watch<TProvider>()
            where TProvider : IProvider;
        TProvider Watch<TProvider>(ProviderKey key)
            where TProvider : IProvider;
        TProvider Watch<TProvider>(ProviderKeyOf<TProvider> key)
            where TProvider : IProvider;
        Subscription Listen<TProvider>()
            where TProvider : IProvider;
        Subscription Listen<TProvider>(ProviderKey key)
            where TProvider : IProvider;
        Subscription Listen<TProvider>(out TProvider provider)
            where TProvider : IProvider;
        Subscription Listen<TProvider>(ProviderKey key, out TProvider provider)
            where TProvider : IProvider;
        Subscription Listen<TProvider>(ProviderKeyOf<TProvider> key, out TProvider provider)
            where TProvider : IProvider;
        Subscription Listen<TProvider>(TProvider provider)
            where TProvider : IProvider;
        void Refresh();
        void Invalidate();
        void Dispatch(IElementEvent evt);
        void DispatchListeners(IElementEvent evt);
    }
}

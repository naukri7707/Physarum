using System;
using System.Collections.Generic;
using System.Linq;

namespace Naukri.Physarum.Core
{
    public class Listener : Context
    {
        internal Listener(Element self)
            : base(self) { }

        internal HashSet<Notifier> notifiers = new();

        #region methods

        public override TProvider Watch<TProvider>()
        {
            var provider = Read<TProvider>();
            Subscribe(provider.Notifier);
            return provider;
        }

        public override TProvider Watch<TProvider>(ProviderKey key)
        {
            var provider = Read<TProvider>(key);
            Subscribe(provider.Notifier);
            return provider;
        }

        public override TProvider Watch<TProvider>(ProviderKeyOf<TProvider> key)
        {
            var provider = Read(key);
            Subscribe(provider.Notifier);
            return provider;
        }

        public override Subscription Listen<TProvider>()
        {
            return Listen<TProvider>(out var _);
        }

        public override Subscription Listen<TProvider>(ProviderKey key)
        {
            return Listen<TProvider>(key, out var _);
        }

        public override Subscription Listen<TProvider>(out TProvider provider)
        {
            provider = Read<TProvider>();
            return Listen(provider);
        }

        public override Subscription Listen<TProvider>(ProviderKey key, out TProvider provider)
        {
            provider = Read<TProvider>(key);
            return Listen(provider);
        }

        public override Subscription Listen<TProvider>(
            ProviderKeyOf<TProvider> key,
            out TProvider provider
        )
        {
            provider = Read(key);
            return Listen(provider);
        }

        public override Subscription Listen<TProvider>(TProvider provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            return new Subscription(provider.Notifier, this);
        }

        public override void DispatchListeners(IElementEvent evt)
        {
            throw new InvalidOperationException($"{GetType().Name} can not trigger notifications.");
        }

        internal void Subscribe(Notifier notifier)
        {
            notifier.listeners.Add(this);
            notifiers.Add(notifier);
        }

        internal void Unsubscribe(Notifier notifier)
        {
            notifier.listeners.Remove(this);
            notifiers.Remove(notifier);
        }

        internal void UnsubscribeAllNotifiers()
        {
            foreach (var notifier in notifiers.ToArray())
            {
                Unsubscribe(notifier);
            }

            notifiers.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnsubscribeAllNotifiers();
            }
        }

        #endregion
    }
}

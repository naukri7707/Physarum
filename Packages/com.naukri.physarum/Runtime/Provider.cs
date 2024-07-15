using System;
using Naukri.Physarum.Core;

namespace Naukri.Physarum
{
    public interface IProvider : IElement
    {
        public ProviderKey Key { get; }
        internal Notifier Notifier { get; }
    }

    public partial class Provider : Element<Notifier>, IProvider
    {
        #region constructors

        public Provider()
        {
            key = ProviderKey.Unique();
            RegisterToContainer(this);
        }

        public Provider(ProviderKey Key)
        {
            key = Key;
            RegisterToContainer(this);
        }

        protected Provider(IProvider provider)
            : base(provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            key = provider.Key;
            RegisterToContainer(provider);
        }

        #endregion

        private readonly ProviderKey key;

        public virtual ProviderKey Key => key;

        Notifier IProvider.Notifier
        {
            get
            {
                if (context == null)
                {
                    throw new InvalidOperationException(
                        $"Failed to get {nameof(Notifier)} from {GetType().Name}."
                    );
                }
                return context;
            }
        }

        #region methods

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (ProviderContainer.TryLocate(out var container))
                {
                    container.Unregister(this);
                }
            }
        }

        private static void RegisterToContainer(IProvider provider)
        {
            var container = ProviderContainer.LocateOrCreate();
            container.Register(provider);
        }

        private protected sealed override Notifier BuildContext()
        {
            return new Notifier(this);
        }

        #endregion
    }
}

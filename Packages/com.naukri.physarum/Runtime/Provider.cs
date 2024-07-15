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
        }

        public Provider(ProviderKey Key)
        {
            key = Key;
        }

        protected Provider(IProvider provider)
            : base(provider)
        {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            key = provider.Key;
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

        protected override void HandleEvent(object sender, IElementEvent evt)
        {
            base.HandleEvent(sender, evt);

            switch (evt)
            {
                case ElementEvents.Enable:
                    RegisterToContainer();
                    break;

                default:
                    break;
            }
        }

        private void RegisterToContainer()
        {
            var container = ProviderContainer.LocateOrCreate();
            container.Register(this);
        }

        private protected sealed override Notifier BuildContext()
        {
            return new Notifier(this);
        }

        #endregion
    }
}

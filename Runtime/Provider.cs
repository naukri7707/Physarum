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
        protected Provider()
        {
            key = ProviderKey.Unique();
        }

        protected Provider(ProviderKey Key)
        {
            key = Key;
        }

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

        private protected sealed override Notifier BuildContext()
        {
            return new Notifier(this);
        }
    }
}

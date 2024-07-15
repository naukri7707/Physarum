using Naukri.Physarum.Core;

namespace Naukri.Physarum
{
    partial class Provider
    {
        public new abstract class Behaviour : Element<Notifier>.Behaviour, IProvider
        {
            public virtual ProviderKey Key => ProviderKey.FromType(GetType());

            Notifier IProvider.Notifier => (Notifier)ctx;

            protected override Element<Notifier> BuildElement()
            {
                return new Provider(Key);
            }
        }
    }
}

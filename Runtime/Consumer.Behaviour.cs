using Naukri.Physarum.Core;

namespace Naukri.Physarum
{
    public partial class Consumer
    {
        public new abstract class Behaviour : Element<Listener>.Behaviour, IConsumer
        {
            Listener IConsumer.Listener => (Listener)ctx;

            protected abstract void Build();

            protected override Element<Listener> BuildElement()
            {
                return new Consumer(Build);
            }
        }
    }
}

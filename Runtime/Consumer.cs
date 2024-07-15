using System;
using Naukri.Physarum.Core;

namespace Naukri.Physarum
{
    public interface IConsumer
    {
        internal Listener Listener { get; }
    }

    public partial class Consumer : Element<Listener>, IConsumer
    {
        public Consumer(Action<IContext> build, bool enable = true)
        {
            this.build = () => build(ctx);

            if (enable)
            {
                Enable();
            }

            EnsureInitialize();
        }

        protected Consumer(Action build)
        {
            this.build = build;
        }

        private readonly Action build;

        Listener IConsumer.Listener
        {
            get
            {
                if (context == null)
                {
                    throw new InvalidOperationException(
                        $"Failed to get {nameof(Listener)} from {GetType().Name}."
                    );
                }
                return context;
            }
        }

        protected override void HandleEvent(object sender, IElementEvent evt)
        {
            base.HandleEvent(sender, evt);

            switch (evt)
            {
                case ElementEvents.Refresh:
                    build();
                    break;

                default:
                    break;
            }
        }

        private protected override Listener BuildContext()
        {
            return new Listener(this);
        }
    }
}

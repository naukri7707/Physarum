using System;
using Naukri.Physarum.Core;

namespace Naukri.Physarum
{
    public interface IConsumer : IElement
    {
        internal Listener Listener { get; }

        internal void Build();
    }

    public partial class Consumer : Element<Listener>, IConsumer
    {
        public Consumer(Action<IContext> build)
        {
            this.build = () => build(ctx);
        }

        protected Consumer(IConsumer consumer)
            : base(consumer)
        {
            _ = consumer ?? throw new ArgumentNullException(nameof(consumer));
            build = consumer.Build;
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

        #region methods
        void IConsumer.Build() => build();

        protected override void HandleEvent(object sender, IElementEvent evt)
        {
            base.HandleEvent(sender, evt);

            switch (evt)
            {
                case ElementEvents.Refresh:
                    build();
                    break;

                case ElementEvents.Invalidate:
                    context.UnsubscribeAllNotifiers();
                    break;

                default:
                    break;
            }
        }

        private protected override Listener BuildContext()
        {
            return new Listener(this);
        }

        #endregion
    }
}

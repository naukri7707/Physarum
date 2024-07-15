using System;
using Naukri.Physarum.Core;

namespace Naukri.Physarum
{
    public sealed class Subscription : IDisposable
    {
        internal Subscription(Notifier notifier, Listener listener)
        {
            this.notifier = notifier;
            this.listener = listener;

            Start();
        }

        private readonly Notifier notifier;
        private readonly Listener listener;

        public void Cancel()
        {
            listener.Unsubscribe(notifier);
        }

        public void Start()
        {
            listener.Subscribe(notifier);
        }

        #region IDisposable

        public void Dispose()
        {
            Cancel();
        }

        #endregion
    }
}

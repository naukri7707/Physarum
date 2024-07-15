using System.Collections.Generic;
using System.Linq;

namespace Naukri.Physarum.Core
{
    public class Notifier : Listener
    {
        internal Notifier(IElement self)
            : base(self) { }

        internal HashSet<Listener> listeners = new();

        public override void DispatchListeners(IElementEvent evt)
        {
            var enabledListeners = listeners.Where(it => it.self.IsEnable).ToArray();
            foreach (var listener in enabledListeners)
            {
                listener.self.HandleEvent(self, evt);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                foreach (var listener in listeners.ToArray())
                {
                    listener.Unsubscribe(this);
                }

                notifiers.Clear();
            }
        }
    }
}

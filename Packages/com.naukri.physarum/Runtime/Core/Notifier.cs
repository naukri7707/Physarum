using System.Collections.Generic;
using System.Linq;

namespace Naukri.Physarum.Core
{
    public class Notifier : Listener
    {
        internal Notifier(Element self)
            : base(self) { }

        internal HashSet<Listener> listeners = new();

        public override void DispatchListeners(IElementEvent evt)
        {
            foreach (var listener in listeners)
            {
                Element.DispatchEvent(listener.element, element, evt);
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

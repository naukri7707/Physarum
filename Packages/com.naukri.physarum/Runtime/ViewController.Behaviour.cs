using System;
using Naukri.Physarum.Core;

namespace Naukri.Physarum
{
    public partial class ViewController<TState>
        where TState : IEquatable<TState>
    {
        public new abstract class Behaviour
            : StateProvider<TState>.Behaviour,
                IViewController<TState>
        {
            protected override Element<Notifier> BuildElement()
            {
                return new ViewController<TState>(this);
            }
        }
    }
}

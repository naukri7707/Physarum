using System;
using System.Threading.Tasks;
using Naukri.Physarum.Core;

namespace Naukri.Physarum
{
    public partial class StateProvider<TState>
        where TState : IEquatable<TState>
    {
        public new abstract class Behaviour : Provider.Behaviour, IStateProvider<TState>
        {
            private StateProvider<TState> StateProvider => (StateProvider<TState>)Element;

            public TState State => StateProvider.State;

            #region methods

            public bool SetState(TState state) => StateProvider.SetState(state);

            public bool SetState(Func<TState, TState> update) => StateProvider.SetState(update);

            public async Task<bool> SetStateAsync(Func<TState, Task<TState>> update) =>
                await StateProvider.SetStateAsync(update);

            protected abstract TState Build();

            protected override Element<Notifier> BuildElement()
            {
                return new StateProvider<TState>(Build, Key);
            }
            #endregion
        }
    }
}

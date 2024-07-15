using System;
using System.Threading.Tasks;

namespace Naukri.Physarum
{
    public interface IStateProvider<TState> : IProvider
        where TState : IEquatable<TState>
    {
        public TState State { get; }

        public bool SetState(TState state);
        public bool SetState(Func<TState, TState> update);
        public Task<bool> SetStateAsync(Func<TState, Task<TState>> update);
    }

    public partial class StateProvider<TState> : Provider, IStateProvider<TState>
        where TState : IEquatable<TState>
    {
        #region constructors

        public StateProvider(Func<IContext, TState> build, bool enable = true)
        {
            this.build = () => build(ctx);
            if (enable)
            {
                Enable();
            }

            EnsureInitialize();
        }

        public StateProvider(Func<IContext, TState> build, ProviderKey key, bool enable = true)
            : base(key)
        {
            this.build = () => build(ctx);
            if (enable)
            {
                Enable();
            }

            EnsureInitialize();
        }

        protected StateProvider(Func<TState> build)
        {
            this.build = build;
        }

        protected StateProvider(Func<TState> build, ProviderKey key)
            : base(key)
        {
            this.build = build;
        }

        #endregion

        private TState state;
        private readonly Func<TState> build;

        public TState State
        {
            get
            {
                AssertIsActive();
                return state;
            }
        }

        #region methods

        public bool SetState(TState state)
        {
            AssertIsActive();
            return SetStateImpl(state);
        }

        public bool SetState(Func<TState, TState> update)
        {
            AssertIsActive();
            _ = update ?? throw new ArgumentNullException(nameof(update));

            var oldState = state;
            var newState = update(oldState);
            return SetStateImpl(newState);
        }

        public async Task<bool> SetStateAsync(Func<TState, Task<TState>> update)
        {
            AssertIsActive();
            _ = update ?? throw new ArgumentNullException(nameof(update));

            var oldState = state;
            var newState = await update(oldState);
            return SetStateImpl(newState);
        }

        protected override void HandleEvent(object sender, IElementEvent evt)
        {
            base.HandleEvent(sender, evt);
            switch (evt)
            {
                case ElementEvents.Refresh:
                    var newState = build();
                    SetState(newState);
                    break;

                case ElementEvents.StateChanged:
                    ctx.DispatchListeners(ElementEvents.Refresh.Default);
                    break;

                default:
                    break;
            }
        }

        private bool SetStateImpl(TState state)
        {
            var oldState = this.state;

            if (!Equals(oldState, state))
            {
                this.state = state;
                ctx.Dispatch(ElementEvents.StateChanged.Default);
                return true;
            }

            return false;
        }

        #endregion
    }
}

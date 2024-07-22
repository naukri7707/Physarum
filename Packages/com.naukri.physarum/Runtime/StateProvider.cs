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
        internal TState Build();
    }

    public partial class StateProvider<TState> : Provider, IStateProvider<TState>
        where TState : IEquatable<TState>
    {
        #region constructors

        public StateProvider(Func<IContext, TState> build)
        {
            this.build = () => build(ctx);
        }

        public StateProvider(Func<IContext, TState> build, ProviderKey key)
            : base(key)
        {
            this.build = () => build(ctx);
        }

        protected StateProvider(IStateProvider<TState> stateProvider)
            : base(stateProvider)
        {
            _ = stateProvider ?? throw new ArgumentNullException(nameof(stateProvider));
            build = stateProvider.Build;
        }

        #endregion

        private TState state;
        private readonly Func<TState> build;

        public TState State
        {
            get
            {
                AssertValidated();
                return state;
            }
        }

        #region methods

        public bool SetState(TState state)
        {
            AssertValidated();
            return SetStateImpl(state);
        }

        public bool SetState(Func<TState, TState> update)
        {
            _ = update ?? throw new ArgumentNullException(nameof(update));
            AssertValidated();

            var oldState = state;
            var newState = update(oldState);
            return SetStateImpl(newState);
        }

        public async Task<bool> SetStateAsync(Func<TState, Task<TState>> update)
        {
            _ = update ?? throw new ArgumentNullException(nameof(update));
            AssertValidated();

            var oldState = state;
            var newState = await update(oldState);
            return SetStateImpl(newState);
        }

        TState IStateProvider<TState>.Build() => build();

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

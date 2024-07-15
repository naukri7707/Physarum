using System;

namespace Naukri.Physarum
{
    public interface IViewController<TState> : IStateProvider<TState>
        where TState : IEquatable<TState> { }

    public partial class ViewController<TState> : StateProvider<TState>, IViewController<TState>
        where TState : IEquatable<TState>
    {
        #region constructors

        public ViewController(Func<IContext, TState> build)
            : base(build) { }

        public ViewController(Func<IContext, TState> build, ProviderKey key)
            : base(build, key) { }

        protected ViewController(IViewController<TState> viewController)
            : base(viewController) { }

        #endregion

        protected override void HandleEvent(object sender, IElementEvent evt)
        {
            base.HandleEvent(sender, evt);
            switch (evt)
            {
                case ElementEvents.StateChanged:
                    ctx.Dispatch(ElementEvents.Refresh.Default);
                    break;

                default:
                    break;
            }
        }
    }
}

using System;

namespace Naukri.Physarum
{
    public interface IViewController { }

    public partial class ViewController<TState> : StateProvider<TState>, IViewController
        where TState : IEquatable<TState>
    {
        #region constructors

        public ViewController(Func<IContext, TState> build, bool enable = true)
            : base(build)
        {
            if (enable)
            {
                Enable();
            }
            EnsureInitialize();
        }

        public ViewController(Func<IContext, TState> build, ProviderKey key, bool enable = true)
            : base(build, key)
        {
            if (enable)
            {
                Enable();
            }
            EnsureInitialize();
        }

        protected ViewController(Func<TState> build)
            : base(build) { }

        protected ViewController(Func<TState> build, ProviderKey key)
            : base(build, key) { }

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

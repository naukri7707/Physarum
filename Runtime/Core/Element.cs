using System;
using System.Diagnostics.CodeAnalysis;
using Naukri.Physarum.Extensions;

namespace Naukri.Physarum.Core
{
    public interface IElement
    {
        public bool IsInitialized { get; }
        public bool IsEnable { get; }

        [SuppressMessage(
            "Style",
            "IDE1006",
            Justification = "Lowercase 'ctx' intentionally used as field-like accessor"
        )]
        protected IContext ctx { get; }

        public void Enable();
        public void Disable();
        internal void HandleEvent(object sender, IElementEvent evt);
        internal void EnsureInitialize();
        private protected abstract IContext BuildContext();
    }

    public abstract partial class Element<TContext> : IElement, IDisposable
        where TContext : IContext
    {
        protected Element()
        {
            context = BuildContext();
        }

        #region fields
        private bool isInitialized;
        private bool isEnable;
        private protected readonly TContext context;
        #endregion

        #region properties
        public bool IsInitialized => isInitialized;
        public bool IsEnable => isEnable;

        [SuppressMessage(
            "Style",
            "IDE1006",
            Justification = "Lowercase 'ctx' intentionally used as field-like accessor"
        )]
        protected IContext ctx
        {
            get
            {
                if (context == null)
                {
                    throw new InvalidOperationException(
                        $"Failed to get {typeof(TContext).Name} from {GetType().Name}."
                    );
                }
                return context;
            }
        }

        IContext IElement.ctx => ctx;
        #endregion

        #region methods

        public void Enable()
        {
            if (isEnable)
            {
                return;
            }

            isEnable = true;
            ctx.Dispatch(ElementEvents.Enable.Default);
        }

        public void Disable()
        {
            if (!isEnable)
            {
                return;
            }

            isEnable = false;
            ctx.Dispatch(ElementEvents.Disable.Default);
        }

        void IElement.HandleEvent(object sender, IElementEvent evt) => HandleEvent(sender, evt);

        IContext IElement.BuildContext() => BuildContext();

        void IElement.EnsureInitialize() => EnsureInitialize();

        protected void AssertIsActive()
        {
            if (!IsEnable || !IsInitialized)
            {
                throw new InvalidOperationException(
                    $"{GetType().GetFriendlyGenericName()} is not active."
                );
            }
        }

        protected virtual void HandleEvent(object sender, IElementEvent evt)
        {
            switch (evt)
            {
                case ElementEvents.Enable:
                    // Only dispatch refresh if the element has already started
                    // This effectively handles scenarios like scene loading where a large number of Elements are loaded instantaneously
                    // It addresses cases where Consumers are activated but Providers haven't been constructed yet (or hasn't awake in UnityObject)
                    // The Refresh event (which triggers the Build method) will not dispatch (but still dispatch on Start)
                    // So all of the elements build actions will delayed until the Start phase
                    // By this time, all activated Elements are guaranteed to have completed construction (Awake)
                    if (IsInitialized)
                    {
                        ctx.Dispatch(ElementEvents.Refresh.Default);
                    }
                    break;

                case ElementEvents.Initialize:
                    ctx.Dispatch(ElementEvents.Refresh.Default);
                    break;

                default:
                    break;
            }
        }

        private protected void EnsureInitialize()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                ctx.Dispatch(ElementEvents.Initialize.Default);
            }
        }

        private protected abstract TContext BuildContext();
        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ctx.Dispose();
            }
        }

        #endregion
    }
}

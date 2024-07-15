using System;
using System.Diagnostics.CodeAnalysis;
using Naukri.Physarum.Extensions;

namespace Naukri.Physarum.Core
{
    public interface IElement
    {
        public bool IsInitialized { get; }
        public bool IsEnable { get; }

        public void Enable();
        public void Disable();
        public void Post(Action<IContext> action);
        internal void HandleEvent(object sender, IElementEvent evt);
    }

    public abstract partial class Element : IElement, IDisposable
    {
        #region constructors

        protected Element()
        {
            eventHandler += HandleEvent;
        }

        protected Element(EventHandler<IElementEvent> eventHandler)
        {
            this.eventHandler += HandleEvent;
            this.eventHandler += eventHandler;
        }

        protected Element(IElement element)
        {
            _ = element ?? throw new ArgumentNullException(nameof(element));
            eventHandler += HandleEvent;
            eventHandler += element.HandleEvent;
        }

        #endregion

        private readonly EventHandler<IElementEvent> eventHandler;

        public abstract bool IsInitialized { get; }
        public abstract bool IsEnable { get; }

        #region methods
        public abstract void Enable();

        public abstract void Disable();

        public abstract void Post(Action<IContext> action);

        void IElement.HandleEvent(object sender, IElementEvent evt) => HandleEvent(sender, evt);

        internal abstract void EnsureInitialize();

        internal static void DispatchEvent(Element element, IElementEvent evt)
        {
            element.eventHandler?.Invoke(element, evt);
        }

        protected void EnsureActive()
        {
            if (!IsEnable)
            {
                throw new InvalidOperationException(
                    $"{GetType().GetFriendlyGenericName()} is not active."
                );
            }
            else
            {
                if (!IsInitialized)
                {
                    EnsureInitialize();
                }
            }
        }

        protected abstract void HandleEvent(object sender, IElementEvent evt);
        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
        #endregion
    }

    public abstract partial class Element<TContext> : Element
        where TContext : IContext
    {
        #region constructors

        protected Element()
        {
            context = BuildContext();
        }

        protected Element(EventHandler<IElementEvent> eventHandler)
            : base(eventHandler)
        {
            context = BuildContext();
        }

        protected Element(IElement element)
            : base(element)
        {
            context = BuildContext();
        }

        #endregion

        #region fields
        private bool isInitialized;
        private bool isEnable;
        private protected readonly TContext context;
        #endregion

        #region properties
        public override sealed bool IsInitialized => isInitialized;
        public sealed override bool IsEnable => isEnable;

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

        #endregion

        #region methods

        public override sealed void Enable()
        {
            if (isEnable)
            {
                return;
            }

            isEnable = true;
            ctx.Dispatch(ElementEvents.Enable.Default);
        }

        public sealed override void Disable()
        {
            if (!isEnable)
            {
                return;
            }

            isEnable = false;
            ctx.Dispatch(ElementEvents.Disable.Default);
        }

        public override void Post(Action<IContext> action)
        {
            action(ctx);
        }

        internal sealed override void EnsureInitialize()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                ctx.Dispatch(ElementEvents.Initialize.Default);
            }
        }

        protected override void HandleEvent(object sender, IElementEvent evt)
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

        private protected abstract TContext BuildContext();
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disable();
                ctx.Dispose();
            }
        }

        #endregion
    }
}

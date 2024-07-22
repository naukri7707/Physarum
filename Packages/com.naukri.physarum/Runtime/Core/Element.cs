using System;
using System.Diagnostics.CodeAnalysis;
using Naukri.Physarum.Extensions;

namespace Naukri.Physarum.Core
{
    public interface IElement
    {
        public bool IsValidated { get; }

        public void Post(Action<IContext> action);
        internal void HandleEvent(object sender, IElementEvent evt);
        internal void EnsureValidated();
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
        private bool isValidated;

        public bool IsValidated => isValidated;

        #region methods
        public abstract void Post(Action<IContext> action);

        public void AssertValidated()
        {
            if (!IsValidated)
            {
                throw new InvalidOperationException(
                    $"{GetType().GetFriendlyGenericName()} was invalidated. You must refresh it first."
                );
            }
        }

        void IElement.HandleEvent(object sender, IElementEvent evt) => HandleEvent(sender, evt);

        void IElement.EnsureValidated() => EnsureValidated();

        internal abstract void EnsureValidated();

        internal static void DispatchEvent(Element element, object sender, IElementEvent evt)
        {
            element.eventHandler?.Invoke(sender, evt);
        }

        protected virtual void HandleEvent(object sender, IElementEvent evt)
        {
            switch (evt)
            {
                case ElementEvents.Invalidate:
                    isValidated = false;
                    break;

                case ElementEvents.Refresh:
                    isValidated = true;
                    break;

                default:
                    break;
            }
        }

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

        private protected readonly TContext context;

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

        #region methods

        public override void Post(Action<IContext> action)
        {
            action(ctx);
        }

        internal override void EnsureValidated()
        {
            if (!IsValidated)
            {
                ctx.Refresh();
            }
        }

        private protected abstract TContext BuildContext();
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ctx.Dispose();
            }
        }

        #endregion
    }
}

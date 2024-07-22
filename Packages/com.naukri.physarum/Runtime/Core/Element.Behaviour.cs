using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Naukri.Physarum.Core
{
    public abstract partial class Element<TContext>
    {
        public abstract class Behaviour : MonoBehaviour, IElement
        {
            private Element<TContext> element;

            #region properties
            public bool IsValidated => Element.IsValidated;

            [SuppressMessage(
                "Style",
                "IDE1006",
                Justification = "Lowercase 'ctx' intentionally used as field-like accessor"
            )]
            protected IContext ctx => Element.ctx;

            private protected Element<TContext> Element
            {
                get
                {
                    if (element == null)
                    {
                        throw new InvalidOperationException(
                            $"Failed to get element from {GetType().Name}. Ensure gameObject '{name}' is awake already."
                        );
                    }
                    return element;
                }
            }

            #endregion

            #region methods
            public void Post(Action<IContext> action) => Element.Post(action);

            void IElement.HandleEvent(object sender, IElementEvent evt) => HandleEvent(sender, evt);

            void IElement.EnsureValidated() => Element.EnsureValidated();

            protected virtual void Awake() => element = BuildElement();

            protected virtual void OnDestroy() => Element.Dispose();

            // Refresh when Element is mounted instead of Consumer
            // This is because there are some cases like StateProvider hasn't been listened yet but self-invokes
            protected virtual void OnEnable()
            {
                if (didStart)
                {
                    Element.EnsureValidated();
                }
            }

            protected virtual void OnDisable()
            {
                ctx.Invalidate();
            }

            protected virtual void Start()
            {
                Element.EnsureValidated();
            }

            protected virtual void HandleEvent(object sender, IElementEvent evt) { }

            protected abstract Element<TContext> BuildElement();
            #endregion
        }
    }
}

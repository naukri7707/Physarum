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
            public bool IsInitialized => Element.IsInitialized;
            public bool IsEnable => Element.IsEnable;

            [SuppressMessage(
                "Style",
                "IDE1006",
                Justification = "Lowercase 'ctx' intentionally used as field-like accessor"
            )]
            protected IContext ctx
            {
                get
                {
                    var e =
                        Element
                        ?? throw new InvalidOperationException(
                            $"Failed to get Element from {GetType().Name}. Ensure gameObject '{name}' is awake already."
                        );

                    return e.ctx;
                }
            }

            private protected Element<TContext> Element => element;
            #endregion

            #region methods
            public void Enable() => enabled = true;

            public void Disable() => enabled = false;

            public void Post(Action<IContext> action) => Element.Post(action);

            void IElement.HandleEvent(object sender, IElementEvent evt) => HandleEvent(sender, evt);

            protected virtual void Awake() => element = BuildElement();

            protected virtual void OnDestroy() => Element.Dispose();

            protected virtual void OnEnable() => Element.Enable();

            protected virtual void OnDisable() => Element.Disable();

            protected virtual void Start() => Element.EnsureInitialize();

            protected virtual void HandleEvent(object sender, IElementEvent evt) { }

            protected abstract Element<TContext> BuildElement();
            #endregion
        }
    }
}

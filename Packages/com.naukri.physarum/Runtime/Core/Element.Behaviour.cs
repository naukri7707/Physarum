using System;
using UnityEngine;

namespace Naukri.Physarum.Core
{
    public abstract partial class Element<TContext>
        where TContext : IContext
    {
        public abstract class Behaviour : MonoBehaviour, IElement
        {
            private Element<TContext> element;

            #region properties
            public bool IsInitialized => Element.IsInitialized;
            public bool IsEnable => Element.IsEnable;

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

            IContext IElement.ctx => Element.ctx;
            private protected Element<TContext> Element => element;
            #endregion

            #region methods
            public void Enable() => enabled = true;

            public void Disable() => enabled = false;

            IContext IElement.BuildContext() => Element.BuildContext();

            void IElement.EnsureInitialize() => Element.EnsureInitialize();

            void IElement.HandleEvent(object sender, IElementEvent evt) =>
                Element.HandleEvent(sender, evt);

            protected virtual void Awake() => element = BuildElement();

            protected virtual void OnDestroy() => Element.Dispose();

            protected virtual void OnEnable() => Element.Enable();

            protected virtual void OnDisable() => Element.Disable();

            protected virtual void Start() => Element.EnsureInitialize();

            protected abstract Element<TContext> BuildElement();
            #endregion
        }
    }
}

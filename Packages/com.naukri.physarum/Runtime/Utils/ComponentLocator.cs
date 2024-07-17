using UnityEngine;

namespace Naukri.Physarum.Utils
{
    public interface IComponentCreatedHandler
    {
        public void OnComponentCreated();
    }

    public static class ComponentLocator<T>
        where T : Component
    {
        private static T componentCache;

        #region methods

        public static T FindComponent(
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude
        )
        {
            TryFindComponent(out var component, findObjectsInactive);
            return component;
        }

        public static bool TryFindComponent(
            out T component,
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude
        )
        {
            if (componentCache == null)
            {
                componentCache = FindImpl(findObjectsInactive);
            }

            component = componentCache;
            return componentCache != null;
        }

        public static T FindOrCreateComponent(
            bool dontDestroyOnLoad = false,
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude
        )
        {
            if (!TryFindComponent(out var component, findObjectsInactive))
            {
                var type = typeof(T);
                var go = new GameObject(type.Name, type);

                if (dontDestroyOnLoad)
                {
                    Object.DontDestroyOnLoad(go);
                }

                component = go.GetComponent<T>();
                if (component is IComponentCreatedHandler createdHandler)
                {
                    createdHandler.OnComponentCreated();
                }

                componentCache = component;
                return component;
            }

            return component;
        }

        public static void Invalidate()
        {
            componentCache = null;
        }

        static T FindImpl(FindObjectsInactive findObjectsInactive)
        {
            return Object.FindAnyObjectByType<T>(findObjectsInactive);
        }

        #endregion
    }
}

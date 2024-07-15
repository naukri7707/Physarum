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
            string name = null,
            bool dontDestroyOnLoad = false,
            FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude
        )
        {
            if (componentCache == null)
            {
                componentCache = FindImpl(findObjectsInactive);

                if (componentCache == null)
                {
                    var type = typeof(T);
                    var goName = name ?? type.Name;
                    var go = new GameObject(goName, type);

                    if (dontDestroyOnLoad)
                    {
                        Object.DontDestroyOnLoad(go);
                    }

                    componentCache = go.GetComponent<T>();
                    if (componentCache is IComponentCreatedHandler createdHandler)
                    {
                        createdHandler.OnComponentCreated();
                    }
                }
            }

            return componentCache;
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

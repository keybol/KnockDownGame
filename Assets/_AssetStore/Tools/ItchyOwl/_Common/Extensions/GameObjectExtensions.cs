using UnityEngine;

namespace ItchyOwl.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Gets or adds a component.
        /// Note that this only returns the first component if there are many.
        /// By default does not seek children, but can be told to do so.
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject go, bool seekChildren = false) where T : Component
        {
            T component = seekChildren ? go.GetComponentInChildren<T>(includeInactive: true) : go.GetComponent<T>();
            if (component == null)
            {
                component = go.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// Removes all components of type T found from the children of this game object.
        /// </summary>
        /// <returns>Returns the count of removed components.</returns>
        public static int RemoveComponentsInChildren<T>(this GameObject go, bool isUsedInEditor = false) where T : Component
        {
            return go.transform.RemoveComponentsInChildren<T>(isUsedInEditor);
        }
    }
}

using UnityEngine;
using System.Linq;
using System.Collections.Generic;
namespace ItchyOwl.Extensions
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Finds and returns all components of type T that are found under this game object excluding the components found on the caller itself.
        /// </summary>
        public static IEnumerable<T> GetComponentsOnlyInChildren<T>(this Component c, bool includeInactive = false)
        {
            var components = c.GetComponents<T>();
            return c.GetComponentsInChildren<T>(includeInactive).Where(obj => !components.Contains(obj));
        }

        /// <summary>
        /// Finds and returns all components of type T that are found from parents excluding the components found on the caller itself.
        /// </summary>
        public static IEnumerable<T> GetComponentsOnlyInParents<T>(this Component c, bool includeInactive = false)
        {
            var components = c.GetComponents<T>();
            return c.GetComponentsInParent<T>(includeInactive).Where(obj => !components.Contains(obj));
        }

        /// <summary>
        /// Removes all components of type T found from the children of this component.
        /// </summary>
        /// <returns>Returns the count of removed components.</returns>
        public static int RemoveComponentsInChildren<T>(this Component c, bool isUsedInEditor = false) where T : Component
        {
            int counter = 0;
            foreach (var transform in c.GetComponentsInChildren<Transform>(true))
            {
                var components = transform.GetComponents<T>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (isUsedInEditor) { Object.DestroyImmediate(components[i]); }
                    else { Object.Destroy(components[i]); }
                    counter++;
                }
            }
            return counter;
        }
    }
}


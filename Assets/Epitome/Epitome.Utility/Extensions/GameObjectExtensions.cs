using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Epitome
{
    public static class GameObjectExtensions
    {
        public static bool HasRigidbody(this GameObject self)
        {
            return (self.GetComponent<Rigidbody>() != null);
        }

        public static bool HasAnimation(this GameObject self)
        {
            return (self.GetComponent<Animation>() != null);
        }

        /// <summary>
        /// Gets or adds a component
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            T component = self.GetComponent<T>();
            if (component == null)
            {
                component = self.AddComponent<T>();
            }
            return component;
        }

        public static T GetSafeComponent<T>(this GameObject self) where T : MonoBehaviour
        {
            T component = self.GetComponent<T>();

            if (component == null)
            {
                Debug.LogError("Expected to find component of type " + typeof(T) + " but found none", self);
            }

            return component;
        }

        public static T GetAbstract<T>(this GameObject self) where T : class
        {
            return self.GetComponents<T>().OfType<T>().FirstOrDefault();
        }

        public static T GetInterface<T>(this GameObject self) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
                return null;
            }
            var tmps = self.GetComponents<Component>().OfType<T>();
            if (tmps.Count() == 0) return null;
            return tmps.First();
        }

        public static IEnumerable<T> GetInterfaces<T>(this GameObject self) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
                return Enumerable.Empty<T>();
            }
            return self.GetComponents<Component>().OfType<T>();
        }

        public static List<T> GetInterfaces_Linq<T>(this GameObject self) where T : class
        {
            return self.GetComponents<MonoBehaviour>()
                   .Where(item => item is T)
                   .Select(item => item as T)
                   .ToList();
        }
    }
}
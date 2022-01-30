using System.Collections;
using UnityEngine;

namespace GGJ.Utility
{
    public static class SingletonHelper<T>
        where T : MonoBehaviour
    {
        public static T Singleton { get; private set; }

        public static bool HandleInstanceEnabled(T instance)
        {
            if (!ReferenceEquals(Singleton, null))
            {
                Debug.LogError($"Cannot assign singleton of type \"{typeof(T).Name}\", as it already exists.");
                instance.enabled = false;
                Object.Destroy(instance.gameObject);
                return false;
            }
            Singleton = instance;
            return true;
        }

        public static bool HandleInstanceDisabled(T instance)
        {
            if (!ReferenceEquals(instance, Singleton))
            {
                return false;
            }
            Singleton = null;
            return true;
        }
    }
}

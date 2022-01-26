using System.Collections.Generic;

namespace GGJ.Utility
{
    public static class InstanceTracker<T>
    {
        private static List<T> instancesList = new List<T>();

        public static void Add(T instance)
        {
            instancesList.Add(instance);
        }

        public static void Remove(T instance)
        {
            instancesList.Remove(instance);
        }

        public static void GetInstances(List<T> dest)
        {
            foreach (T instance in instancesList)
            {
                dest.Add(instance);
            }
        }

        public static IReadOnlyList<T> GetInstancesReadOnly()
        {
            return instancesList;
        }
    }
}

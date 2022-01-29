using UnityEngine;
using GGJ.Utility;

namespace GGJ
{
    public class LocalUser : MonoBehaviour
    {
        #region Engine Messages
        private void OnEnable()
        {
            InstanceTracker<LocalUser>.Add(this);
        }

        private void OnDisable()
        {
            InstanceTracker<LocalUser>.Remove(this);
        }
        #endregion

        public static LocalUser firstInstance
        {
            get
            {
                var instances = InstanceTracker<LocalUser>.GetInstancesReadOnly();
                return (instances.Count == 0) ? null : instances[0];
            }
        }
    }
}

using UnityEngine;
using GGJ.Utility;

namespace GGJ
{
    /// <summary>
    /// Hacky class that is checked by the input system for existence to determine whether or not to accept input.
    /// </summary>
    public class PlayerInputSuppressor : MonoBehaviour
    {
        private void OnEnable()
        {
            InstanceTracker<PlayerInputSuppressor>.Add(this);
        }

        private void OnDisable()
        {
            InstanceTracker<PlayerInputSuppressor>.Remove(this);
        }

        public static bool ShouldSuppressPlayerInput => InstanceTracker<PlayerInputSuppressor>.GetInstancesReadOnly().Count > 0;
    }
}

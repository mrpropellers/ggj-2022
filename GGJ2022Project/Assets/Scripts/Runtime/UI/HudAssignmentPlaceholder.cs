using UnityEngine;
using GGJ.Utility;

namespace GGJ.UI
{
    /// <summary>
    /// Placeholder class for telling the HUD which character to observe.
    /// </summary>
    [RequireComponent(typeof(Hud))]
    public class HudAssignmentPlaceholder : MonoBehaviour
    {
        #region Cached Components
        private Hud hud;
        #endregion

        #region Engine Messages
        private void Awake()
        {
            hud = GetComponent<Hud>();
        }

        private void Update()
        {
            Character newObservedCharacter = null;
            LocalUser firstLocalUser = InstanceTracker<LocalUser>.GetFirstInstance();
            if (firstLocalUser)
            {
                if (firstLocalUser.CurrentTurnReceiver)
                {
                    newObservedCharacter = firstLocalUser.CurrentTurnReceiver.PlayerCharacter;
                }
            }
            hud.ObservedCharacter = newObservedCharacter;
        }
        #endregion
    }
}

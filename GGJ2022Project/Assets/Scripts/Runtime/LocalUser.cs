using System.Collections.Generic;
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
            PlayerTurnReceiver.OnPlayerTurnReceiverDiscovered += OnPlayerTurnReceiverDiscovered;
            PlayerTurnReceiver.OnPlayerTurnReceiverLost += OnPlayerTurnReceiverLost;

            var firstTurnReceiver = InstanceTracker<PlayerTurnReceiver>.GetFirstInstance();
            if (!ReferenceEquals(firstTurnReceiver, null))
            {
                OnPlayerTurnReceiverDiscovered(firstTurnReceiver);
            }
        }

        private void OnDisable()
        {
            if (!ReferenceEquals(CurrentTurnReceiver, null))
            {
                OnPlayerTurnReceiverLost(CurrentTurnReceiver);
            }

            PlayerTurnReceiver.OnPlayerTurnReceiverLost -= OnPlayerTurnReceiverLost;
            PlayerTurnReceiver.OnPlayerTurnReceiverDiscovered -= OnPlayerTurnReceiverDiscovered;

            InstanceTracker<LocalUser>.Remove(this);
        }
        #endregion

        // For now, since the player turn receiver spawns from the scene instead of through our own logic, we have to assume there's only one, listen for it to spawn, and take ownership of it from there.
        private void OnPlayerTurnReceiverDiscovered(PlayerTurnReceiver discoveredPlayerTurnReciever)
        {
            CurrentTurnReceiver = CurrentTurnReceiver ?? discoveredPlayerTurnReciever;
        }

        private void OnPlayerTurnReceiverLost(PlayerTurnReceiver lostPlayerTurnReciever)
        {
            if (!ReferenceEquals(lostPlayerTurnReciever, CurrentTurnReceiver))
            {
                return;
            }
            CurrentTurnReceiver = null;
        }

        public PlayerTurnReceiver CurrentTurnReceiver { get; private set; }
    }
}

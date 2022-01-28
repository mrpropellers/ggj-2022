using System.Collections;
using UnityEngine;
using GGJ.Utility;

namespace GGJ
{
    /// <summary>
    /// Base class for behaviors that can take a game turn.
    /// </summary>
    public abstract class TurnReceiver : MonoBehaviour
    {
        public int previousTurnNumber { get; internal set; }

        protected void OnEnable()
        {
            InstanceTracker<TurnReceiver>.Add(this);
        }

        protected void OnDisable()
        {
            InstanceTracker<TurnReceiver>.Remove(this);
        }

        public abstract IEnumerable HandleTurn(Turn incomingTurn);
    }
}

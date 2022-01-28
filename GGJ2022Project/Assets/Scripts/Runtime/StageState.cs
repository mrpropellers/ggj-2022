using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ
{
    public class StageState : MonoBehaviour
    {
        static StageState s_Instance;

        public static StageState Instance
        {
            get
            {
                EnsureInitialized();
                return s_Instance;
            }
        }

        // XXX: No addy third mode-y >:I
        public enum BoardMode
        {
            Physical,
            Spiritual
        }

        public Board ActiveBoard { get; private set; }
        public BoardMode CurrentBoardMode { get; private set; }
        public Character PlayerCharacter { get; private set; }

        void Awake()
        {
            EnsureInitialized();
        }

        static void EnsureInitialized()
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<StageState>();
                if (s_Instance == null)
                {
                    Debug.LogWarning($"No {nameof(StageState)} instance found in scene, creating one.");
                    var go = new GameObject(nameof(StageState));
                    s_Instance = go.AddComponent<StageState>();
                }
                s_Instance.ActiveBoard = FindObjectOfType<Board>();
                s_Instance.PlayerCharacter = FindObjectOfType<PlayerTurnReceiver>().PlayerCharacter;
            }
        }
    }
}

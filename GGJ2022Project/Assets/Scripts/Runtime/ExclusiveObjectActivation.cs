using System;
using UnityEngine;

namespace GGJ
{
    public class ExclusiveObjectActivation : MonoBehaviour
    {
        #region Inspector Parameters
        public UnityEngine.Object[] Group = Array.Empty<UnityEngine.Object>();
        #endregion

        private UnityEngine.Object m_CurrentActiveObject;
        public UnityEngine.Object ActiveObject
        {
            get => m_CurrentActiveObject;
            set
            {
                int objIndex = Array.IndexOf(Group, value);
                if (objIndex < 0)
                {
                    throw new ArgumentException($"Object \"{(value ? value.name : "null")}\" is not a member of group \"{name}\"");
                }
                for (int i = 0; i < objIndex; ++i)
                {
                    SetObjectEnabled(Group[i], false);
                }
                SetObjectEnabled(Group[objIndex], true);
                for (int i = objIndex + 1; i < Group.Length; ++i)
                {
                    SetObjectEnabled(Group[i], false);
                }
            }
        }

        private static void SetObjectEnabled(UnityEngine.Object obj, bool enabled)
        {
            if (!obj)
            {
                return;
            }
            switch (obj)
            {
                case GameObject gameObject:
                {
                    gameObject.SetActive(enabled);
                } break;
                case Behaviour behaviour:
                {
                    behaviour.enabled = enabled;
                } break;
                default:
                {
                    Debug.LogError($"Object \"{(obj ? obj.name : "null")}\" of type \"{obj?.GetType().Name}\" cannot be enabled or disabled.");
                } break;
            }
        }

        private static bool GetObjectEnabled(UnityEngine.Object obj)
        {
            if (!obj)
            {
                return false;
            }
            switch (obj)
            {
                case GameObject gameObject:
                    {
                        return gameObject.activeSelf;
                    } break;
                case Behaviour behaviour:
                    {
                        return behaviour.enabled;
                    } break;
                default:
                    {
                        Debug.LogError($"Object \"{(obj ? obj.name : "null")}\" of type \"{obj?.GetType().Name}\" cannot be enabled or disabled.");
                        return false;
                    } break;
            }
        }

        private int FindFirstEnabled()
        {
            for (int i = 0; i < Group.Length; ++i)
            {
                if (GetObjectEnabled(Group[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        #region Engine Messages
        private void OnEnable()
        {
            if (Group.Length > 0)
            {
                int firstEnabledObjectIndex = FindFirstEnabled();
                ActiveObject = Group[firstEnabledObjectIndex];
            }
        }
        #endregion
    }
}

using UnityEngine;
using GGJ.Utility;

namespace GGJ.Runtime
{
    public class StageInfo : MonoBehaviour
    {
        #region Inspector Fields
        public string nextSceneName;
        #endregion

        #region Engine Messages
        private void OnEnable()
        {
            SingletonHelper<StageInfo>.HandleInstanceEnabled(this);
        }

        private void OnDisable()
        {
            SingletonHelper<StageInfo>.HandleInstanceDisabled(this);
        }
        #endregion
    }
}

using UnityEngine;

namespace GGJ.Utility.Utility
{
    public static class LogHelpers
    {
        public static void LogIfPlaying(string message)
        {
            if (Application.isPlaying)
            {
                Debug.Log(message);
            }
        }
    }
}

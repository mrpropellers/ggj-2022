using System.Collections;

namespace GGJ.Utility.Runtime.Utility
{
    public static class UnityHelpers
    {
        public static void DoCoroutineImmediately(IEnumerator coroutine)
        {
            while (coroutine.MoveNext())
            { }
        }
    }
}

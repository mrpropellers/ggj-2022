using System;
using UnityEngine;

namespace GGJ.Utility
{
    public static class QuickMaths
    {
        public static int IJToIndex(int width, int i, int j)
        {
            return i + j * width;
        }

        public static Vector2Int IndexToIJ(int width, int index)
        {
            var j = index % width;
            var i = index - width * j;
            return new Vector2Int(i, j);
        }

        public static bool VectorIsZero(Vector2 vector)
        {
            return Mathf.Approximately(vector.x, 0) &&
                Mathf.Approximately(vector.y, 0);
        }

    }
}

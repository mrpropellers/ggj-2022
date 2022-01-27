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

        public static Vector2Int QuantizeToDirection(Vector2 floatVector)
        {
            if (Mathf.Approximately(floatVector.x, 0) &&
                Mathf.Approximately(floatVector.y, 0))
            {
                return Vector2Int.zero;
            }
            var xAbs = Mathf.Abs(floatVector.x);
            var yAbs = Mathf.Abs(floatVector.y);
            if (xAbs > yAbs)
            {
                return xAbs > floatVector.x ? Vector2Int.left : Vector2Int.right;
            }

            return yAbs > floatVector.y ? Vector2Int.down : Vector2Int.up;
        }
    }
}

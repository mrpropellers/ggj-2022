using UnityEngine;
using Random = UnityEngine.Random;

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

        public static int GridDistance(Vector2Int a, Vector2Int b)
        {
            var x = System.Math.Abs(a.x - b.x);
            var y = System.Math.Abs(b.y - a.y);
            return x + y;
        }

        public static Vector2 RandomDirection()
        {
            var x = Random.Range(-1f, 1f);
            var y = Random.Range(-1f, 1f);
            if (Mathf.Approximately(x, 0) && Mathf.Approximately(y, 0))
            {
                return Vector2.zero;
            }

            var direction = new Vector2(x, y);
            direction.Normalize();
            return direction;
        }
    }
}

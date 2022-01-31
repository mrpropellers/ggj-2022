#if UNITY_EDITOR 

using UnityEngine;
using UnityEditor;

namespace GGJ.Editor
{
    public class BoardGizmoDrawer
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        static void DrawBoardSpaces(Board board, GizmoType gizmoType)
        {
            if (!board.IsValid)
            {
                return;
            }

            var numNullSpaces = 0;
            var originalColor = Gizmos.color;
            foreach (var space in board.AllSpaces)
            {
                if (space == null)
                {
                    numNullSpaces++;
                    continue;
                }
                var position = board.GetWorldCoordinates(space);

                Color color;
                switch (space.BoardSpaceType)
                {
                    case BoardSpace.Flavor.Unknown:
                        color = Color.magenta;
                        break;
                    case BoardSpace.Flavor.Wall:
                        color = Color.red;
                        break;
                    case BoardSpace.Flavor.Null:
                        color = Color.gray;
                        break;
                    case BoardSpace.Flavor.Normal:
                        color = Color.green;
                        break;
                    default:
                        Debug.LogWarning($"No handling implemented for {space.BoardSpaceType}");
                        color = Color.black;
                        break;
                }

                Gizmos.color = color;
                Gizmos.DrawSphere(position, 0.1f);
            }

            Gizmos.color = originalColor;

            if (board.IsConstructing)
            {
                SceneUpdater.RefreshSceneView();
            }

            if (numNullSpaces > 0)
            {
                //Debug.LogWarning($"Found {numNullSpaces} un-initialized spaces in {board.name}");
            }
        }
    }
}

#endif

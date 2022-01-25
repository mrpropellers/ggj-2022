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
                switch (space.State)
                {
                    case Unknown:
                        color = Color.magenta;
                        break;
                    case Blocked _:
                        color = Color.red;
                        break;
                    case Null _:
                        color = Color.gray;
                        break;
                    case Empty _:
                        color = Color.green;
                        break;
                    default:
                        Debug.LogWarning($"No handling implemented for {space.State.GetType()}");
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

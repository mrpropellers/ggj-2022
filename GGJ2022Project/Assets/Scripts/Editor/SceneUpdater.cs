using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ.Editor
{
    [InitializeOnLoad]
    public class SceneUpdater
    {
        static SceneUpdater()
        {
            //s_EditorWindow = EditorWindow.GetWindow<SceneView>();
            // var boards = GameObject.FindObjectsOfType<Board>();
            // if (boards.Length == 0)
            // {
            //     Debug.Log($"No {nameof(Board)} found in scene.");
            // }
            // else if (boards.Length > 1)
            // {
            //     Debug.Log($"{boards.Length} {nameof(Board)}s found in scene - should there be more than one?");
            // }

            // foreach (var board in boards)
            // {
            //     board.OnBoardSpaceConstructed.AddListener((_, _) => RefreshSceneView());
            // }
        }

        internal static void RefreshSceneView()
        {
            //s_EditorWindow.Repaint();
            var sceneView = EditorWindow.GetWindow<SceneView>();
            // TODO: Should add an editorcoroutine to wait one frame before allowing another repaint call
            if (sceneView)
            {
                sceneView.Repaint();
            }
        }
    }
    //{
    //    BoardSpace m_LastSpaceDrawn;
    //    public static DebugDrawer Instance { get; private set; }


    //    static DebugDrawer()
    //    {
    //        Instance ??= FindObjectOfType<DebugDrawer>() ??
    //            new GameObject("DebugDrawer").AddComponent<DebugDrawer>();
    //    }

    //    void Awake()
    //    {
    //        SetUpEventListeners();
    //    }

    //    void SetUpEventListeners()
    //    {
    //        var boards = FindObjectsOfType<Board>();
    //        if (boards.Length == 0)
    //        {
    //            Debug.Log($"No {nameof(Board)} found in scene.");
    //        }
    //        else if (boards.Length > 1)
    //        {
    //            Debug.Log($"{boards.Length} {nameof(Board)}s found in scene - should there be more than one?");
    //        }

    //        foreach (var board in boards)
    //        {
    //            board.OnBoardSpaceConstructed.AddListener(DrawOnConstruction);
    //        }
    //    }

    //    public void DrawOnConstruction(Board board, BoardSpace newSpace)
    //    {
    //        var gizmoColor = Gizmos.color;
    //        foreach (var space in board.AllSpaces)
    //        {
    //            if (space == null)
    //            {
    //                continue;
    //            }
    //            var position = board.GetWorldCoordinates(space);
    //            if (space != m_LastSpaceDrawn && space != newSpace)
    //            {
    //                Gizmos.color = gizmoColor;
    //                Gizmos.DrawSphere(position, 0.1f);
    //            }
    //            else if (space == m_LastSpaceDrawn)
    //            {
    //                Gizmos.color = Color.yellow;
    //                Gizmos.DrawSphere(position, 0.2f);
    //            }
    //            else if (space != newSpace)
    //            {
    //                Debug.LogError($"Logic is broken - don't know where {space} belongs");
    //            }
    //            else
    //            {
    //                Gizmos.color = Color.green;
    //                Gizmos.DrawSphere(position, 0.3f);
    //            }
    //        }
    //        m_LastSpaceDrawn = newSpace;
    //        Gizmos.color = gizmoColor;
    //    }
    //}
}

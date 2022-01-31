#if UNITY_EDITOR 

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace GGJ.Editor
{
    [EditorTool("Snap To Board", typeof(SpriteRenderer))]
    class BoardSnappingTool : EditorTool
    {
        const string k_Name = "Snap to Board";

        //[SerializeField]
        //GUIContent m_IconContent;

        //[SerializeField]
        //Texture2D m_ToolIcon;
        [SerializeField]
        Board m_TargetBoard;

        void OnEnable()
        {
            //m_IconContent = new GUIContent()
            //{
            //    image = m_ToolIcon,
            //    text = k_Name,
            //    tooltip = k_Name
            //};
        }


        // TODO? Figure out how to get a custom icon working
        //public override GUIContent toolbarIcon => m_IconContent;

        public override void OnActivated()
        {
            var boards = FindObjectsOfType<Board>();
            if (boards.Length > 1)
            {
                Debug.LogWarning($"Multiple {nameof(Board)}s detected - ignoring all but {boards[0].name}");
            }

            if (boards.Length == 0)
            {
                if (FindObjectOfType<Grid>())
                {
                    Debug.LogWarning(
                        $"No {nameof(Board)}s in scene, but there is a Grid - " +
                        $"did you forget to attach the {nameof(Board)}?");
                }
                else
                {
                    Debug.LogWarning($"No {nameof(Board)}s or Grids in scene, this tool is useless here!");
                }

                m_TargetBoard = null;
            }
            else
            {
                m_TargetBoard = boards[0];
                if (m_TargetBoard.NeedsInitialization)
                {
                    EditorCoroutineUtility.StartCoroutine(m_TargetBoard.ConstructBoardSpaces(), m_TargetBoard);
                }

            }
        }

        // Transform handles seem to be drawing at the sprite center of mass instead of its pivot position, so we
        // have to correct for that here...
        Vector3 PivotPosition()
        {
            return Tools.handlePosition - ((SpriteRenderer) target).sprite.bounds.center;
        }

        static bool SpaceCanHoldPiece(BoardSpace space, BoardPiece piece)
        {
            var stageStartingMode = StageState.Instance.StartingBoardMode;
            return StageState.SpaceSupportsHoldingPiece(space, piece, stageStartingMode);
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (m_TargetBoard == null || target is not SpriteRenderer)
            {
                return;
            }

            // TODO? Find a way to notify user that this doesn't work on multiple objects without spamming
            if (Selection.count > 1)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();

            var pivotWorld = PivotPosition();

            // TODO? Get size from Board and have Board enforce that constraint on its Grid
            var size = m_TargetBoard.Grid.cellSize.x * 0.5f;

            Vector3 newPosition;
            using (new Handles.DrawingScope(Color.green))
            {
                newPosition = Handles.Slider2D(
                    pivotWorld, Vector3.forward, Vector3.right, Vector3.up, size,
                    Handles.RectangleHandleCap, 0f);
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (m_TargetBoard.TryGetSpace(newPosition, out var boardSpace) && boardSpace.CanHoldTangible)
                {
                    Undo.RecordObjects(Selection.transforms, "Move Piece");
                    var position = m_TargetBoard.GetWorldCoordinates(boardSpace);
                    Selection.transforms[0].position = position;
                    if (((SpriteRenderer) target).TryGetComponent<BoardPiece>(out var piece) &&
                        SpaceCanHoldPiece(boardSpace, piece))
                    {
                        m_TargetBoard.PlacePiece(piece, boardSpace);
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"{target.name} does not have a {nameof(BoardPiece)} component attached - should it?");
                    }
                }
            }
        }
    }
}

#endif

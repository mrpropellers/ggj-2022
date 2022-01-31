#if UNITY_EDITOR 
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using UnityEngine;


namespace GGJ.Editor.CustomEditors
{
    [CustomEditor(typeof(Board))]
    public class BoardEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var board = target as Board;
            if (ConstructBoardButton() && !board.IsConstructing)
            {
                EditorCoroutineUtility.StartCoroutine(
                    board.ConstructBoardSpaces(
                        // Small enough WaitForSeconds is basically the same as "WaitForRepaint"
                        new EditorWaitForSeconds(0.01f)), board);
            }
        }

        public bool ConstructBoardButton()
        {
            return GUILayout.Button("Construct Board");
        }
    }
}

#endif

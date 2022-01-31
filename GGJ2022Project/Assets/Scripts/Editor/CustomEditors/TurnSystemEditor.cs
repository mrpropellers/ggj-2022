#if UNITY_EDITOR 

using UnityEditor;
using UnityEngine;

namespace GGJ.Editor.CustomEditors
{
    [CustomEditor(typeof(Board))]
    public class TurnSystemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            // var turnSystem = target as TurnSystem;
            // if (ForceTurnAdvanceButton())
            // {

            // }
        }

        public bool ForceTurnAdvanceButton()
        {
            return GUILayout.Button("Force Next Turn");
        }
    }
}

#endif

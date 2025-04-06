using qiekn.core;
using UnityEditor;
using UnityEngine;

namespace qiekn.editor {
    [CustomEditor(typeof(GridManager))]
    public class GridManagerEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var obj = target as GridManager;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Print crateCells")) {
                obj.DebugPrint();
            }
            GUILayout.EndHorizontal();
        }

    }
}

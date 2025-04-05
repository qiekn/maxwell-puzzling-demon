using UnityEngine;
using UnityEditor;

namespace qiekn.core {
    [CustomEditor(typeof(LevelManager))]
    public class LevelEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var tm = (LevelManager)target;

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save Level")) { tm.SaveLevel(); }
            if (GUILayout.Button("Edit Level")) { tm.EditLevel(); }
            if (GUILayout.Button("Clear Level")) { tm.ClearLevel(); }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Enter")) { tm.PlayerEnter(); }
            if (GUILayout.Button("Exit")) { tm.PlayerExit(); }

            GUILayout.EndHorizontal();
        }
    }
}

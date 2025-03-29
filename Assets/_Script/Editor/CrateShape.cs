using qiekn.core;
using UnityEditor;
using UnityEngine;

namespace qiekn.editor {
    [CustomEditor(typeof(Crate))]
    public class CrateShape : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var obj = target as Crate;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Sprite")) { obj.GenerateSprite(); }
            if (GUILayout.Button("Update Color")) { obj.UpdateColor(); }
            GUILayout.EndHorizontal();
        }

    }
}

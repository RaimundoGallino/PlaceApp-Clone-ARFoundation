using UnityEngine;
using UnityEditor;
using System;


namespace DimBoxes
{
    [CustomEditor(typeof(BoundBoxProgressive))]

    public class BoundBoxProgressiveEditor : Editor
    {
        override public void OnInspectorGUI()
        {
            var BoundBox = target as BoundBoxProgressive;
            DrawDefaultInspector();
            serializedObject.Update();

            using (var gl_group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(BoundBox.wire_renderer)))
            {
                if (gl_group.visible == true)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("gl_Color");
                    BoundBox.wireColor = EditorGUILayout.ColorField(BoundBox.wireColor);
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Line_renderer");
            Undo.RecordObject(BoundBox, (BoundBox.line_renderer? "Enabling" : "Disabling") + " line_renderer");
            BoundBox.line_renderer = EditorGUILayout.Toggle(BoundBox.line_renderer);
            EditorGUILayout.EndHorizontal();

            using (var lr_group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(BoundBox.line_renderer)))
            {
                if (lr_group.visible == true)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("linePrefab");
                    BoundBox.linePrefab = EditorGUILayout.ObjectField(BoundBox.linePrefab, typeof(UnityEngine.Object), true);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("lineWidth");
                    BoundBox.lineWidth = EditorGUILayout.Slider(BoundBox.lineWidth,0.005f, 0.25f);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("lineColor");
                    BoundBox.lineColor = EditorGUILayout.ColorField(BoundBox.lineColor);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("numCapVertices");
                    BoundBox.numCapVertices = EditorGUILayout.IntField(BoundBox.numCapVertices);
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;
                }
            }
            //EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);
            string[] enum_names = Enum.GetNames(typeof(Animation_mode));
            int[] enum_values = (int[])Enum.GetValues(typeof(Animation_mode));
            BoundBox.anim_mode = EditorGUILayout.IntPopup("Animation_mode",BoundBox.anim_mode, enum_names, enum_values);
            //EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.PrefixLabel("progress");
            BoundBox.Progress = EditorGUILayout.Slider("Progress", BoundBox.Progress, 0f, 1.0f);
            //EditorGUILayout.EndHorizontal();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                BoundBox.OnValidate();
            }
        }
    }
}

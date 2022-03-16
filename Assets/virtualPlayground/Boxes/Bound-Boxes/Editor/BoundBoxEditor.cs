using UnityEngine;
using UnityEditor;
using System;

namespace DimBoxes
{
    [CustomEditor(typeof(BoundBox))]

    public class BoundBoxEditor : Editor
    {
        override public void OnInspectorGUI()
        {
            var BoundBox = target as BoundBox;
            base.DrawDefaultInspector();
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
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                BoundBox.OnValidate();
            }
        }
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace DimBoxes
{
    [CustomPropertyDrawer(typeof(DimBox.PlacementOptions))]
    class PlacementOptionDrawer : PropertyDrawer
    {
        //public bool showProperty = true;
        float height = 0f;
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            height = 0f;
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property. 
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y + 3, position.width - 6, 15), property.isExpanded, label,  EditorStyles.foldout);
            height += 20;
            //public static bool Foldout(Rect position, bool foldout, string content, GUIStyle style = EditorStyles.foldout);
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (property.isExpanded)
            {
                // Don't make child fields be indented
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                // Calculate rects
                DimBox db = property.serializedObject.targetObject as DimBox;
                Rect floatingRect = new Rect(position.x, position.y + height, position.width, 20); height += 20;

                Rect hRect = new Rect(position.x, position.y + height, position.width, 20); height += 20;
                Rect _hRect = new Rect();
                if (!db.faceCamera.height) { _hRect = new Rect(position.x, position.y + height, position.width, 20); height += 20; }
                Rect dRect = new Rect(position.x, position.y + height, position.width, 20); height += 20;
                Rect _dRect = new Rect();
                if (!db.faceCamera.depth) { _dRect = new Rect(position.x, position.y + height, position.width, 20); height += 20; }
                Rect wRect = new Rect(position.x, position.y + height, position.width, 20); height += 20;
                Rect _wRect = new Rect();
                if (!db.faceCamera.width) { _wRect = new Rect(position.x, position.y + height, position.width, 20); height += 20; }

                // Draw fields - passs GUIContent.none to each so they are drawn without labels
                SerializedProperty m_floating = property.FindPropertyRelative("floating");
                SerializedProperty m_h_placement = property.FindPropertyRelative("_heightPlacement");
                SerializedProperty m_h_flip = property.FindPropertyRelative("h_flip");
                SerializedProperty m_d_placement = property.FindPropertyRelative("_depthPlacement");
                SerializedProperty m_d_flip = property.FindPropertyRelative("d_flip");
                SerializedProperty m_w_placement = property.FindPropertyRelative("_widthPlacement");
                SerializedProperty m_w_flip = property.FindPropertyRelative("w_flip");

                EditorGUI.PropertyField(floatingRect, m_floating, new GUIContent("floating"));//EditorGUILayout.PropertyField(m_VectorProp, new GUIContent("Vector Object"));
                if (!m_floating.boolValue)
                {
                    EditorGUI.PropertyField(hRect, m_h_placement, new GUIContent("h_placement"));
                    if (!db.faceCamera.height)
                    {
                        EditorGUI.PropertyField(_hRect, m_h_flip, new GUIContent("h_flip"));
                    }
                    EditorGUI.PropertyField(dRect, m_d_placement, new GUIContent("d_placement"));
                    if (!db.faceCamera.depth)
                    {
                        EditorGUI.PropertyField(_dRect, m_d_flip, new GUIContent("d_flip"));
                    }
                    EditorGUI.PropertyField(wRect, m_w_placement, new GUIContent("w_placement"));
                    if (!db.faceCamera.width)
                    {
                        EditorGUI.PropertyField(_wRect, m_w_flip, new GUIContent("w_flip"));
                    }
                }
                else
                {
                    height = 40;
                }
                // Set indent back to what it was
                EditorGUI.indentLevel = indent;
            }

            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return height;
        }

    }
}

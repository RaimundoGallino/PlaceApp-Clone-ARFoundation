using UnityEngine;
using UnityEditor;


namespace DimBoxes
{
    [CustomEditor(typeof(DimBox))]
    public class DimBoxEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DimBox dimboxScript = (DimBox)target;
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Recalculate Bounds"))
            {
                dimboxScript.AccurateBounds();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
#region TextMeshPro
            /*GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = !dimboxScript.useTextMeshPro;
            if (GUILayout.Button("Switch to TextMeshPro"))
            {
                dimboxScript.SwitchToTMPro();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = dimboxScript.useTextMeshPro;
            if (GUILayout.Button("Switch to TextMesh"))
            {
                dimboxScript.SwitchToTextMesh();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();*/
 #endregion
        }
    }
}
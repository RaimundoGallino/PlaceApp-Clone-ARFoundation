using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBounds : MonoBehaviour
{
    public bool _bIsSelected = true;
    public Color gizmoColor = Color.magenta;
    
    void OnDrawGizmos()
    {
        if (_bIsSelected)
            OnDrawGizmosSelected();
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        foreach(var mf in GetComponentsInChildren<MeshFilter>())
        {
            Gizmos.matrix = mf.transform.localToWorldMatrix;
            Mesh m = mf.sharedMesh;
            Gizmos.DrawWireCube( m.bounds.center, m.bounds.size );
            Gizmos.DrawWireSphere(m.bounds.center, 0.5f);
        }
    }
}

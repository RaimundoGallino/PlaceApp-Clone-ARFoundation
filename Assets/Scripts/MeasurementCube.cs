using UnityEngine;

public class MeasurementCube : MonoBehaviour
{
    [SerializeField] private GameObject model;
    void Start()
    {
        // Parent object
        Mesh m = model.GetComponent<MeshFilter>().sharedMesh;
        
        transform.rotation = model.transform.rotation;
        transform.localScale = m.bounds.size;
        transform.position = model.GetComponent<Renderer>().bounds.center;
    }
}

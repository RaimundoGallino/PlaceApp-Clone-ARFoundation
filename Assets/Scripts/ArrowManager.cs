using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    [SerializeField]
    private Transform _arrowPivot;

    private void OnDrawGizmos()
    {
        transform.position = _arrowPivot.position;
        transform.rotation = _arrowPivot.rotation;
    }

    void Update()
    {
        transform.position = _arrowPivot.position;
        transform.rotation = _arrowPivot.rotation;
    }
}

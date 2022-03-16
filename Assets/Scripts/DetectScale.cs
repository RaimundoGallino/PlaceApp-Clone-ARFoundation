using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectScale : MonoBehaviour
{
    public Text xMeasurement;
    public Text yMeasurement;
    public Text zMeasurement;

    private GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
        xMeasurement.text = $"x: {parent.transform.localScale.x.ToString()}";
        yMeasurement.text = $"y: {parent.transform.localScale.y.ToString()}";
        zMeasurement.text = $"z: {parent.transform.localScale.z.ToString()}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

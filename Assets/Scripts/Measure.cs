using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Measure : MonoBehaviour
{
    [Header("Arrows")]
    [SerializeField] private GameObject arrowL;
    [SerializeField] private GameObject arrowR;
    
    [Range(-0.15f, 0.20f)]
    [SerializeField] private float arrowScale = 0.15f;
    
    [Range(0, 90)]
    [SerializeField] private float arrowAngle = 0f;
    // [SerializeField] private Color arrowColor;
    
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private Color textColor;
    
    [Range(0, 1)]
    [SerializeField] private float textScale;
    
    [Header("Canvas")]
    [SerializeField] private GameObject canvas;
    private float distance;

    void OnDrawGizmos()
    {
        MeasureDistance();
    }

    // private void Start()
    // {
    //     MeasureDistance();
    // }

    // private void OnValidate()
    // {
    //     MeasureDistance();
    // }

    void MeasureDistance()
    {
        distance = Vector3.Distance(arrowL.transform.position, arrowR.transform.position);
        textField.text = distance.ToString("N2") + "m";
        canvas.transform.position = LerpByDistance(arrowL.transform.position, arrowR.transform.position, 0.5f);

        if (arrowL != null)
        {
            // arrowL.GetComponent<SpriteRenderer>().color = arrowColor;
            arrowL.transform.localScale = new Vector3(arrowScale, arrowScale, arrowScale);
            arrowL.transform.localRotation = Quaternion.Euler(arrowAngle, 0, 0);
        }
        if (arrowR != null)
        {
            // arrowR.GetComponent<SpriteRenderer>().color = arrowColor;
            arrowR.transform.localScale = new Vector3(arrowScale, arrowScale, arrowScale);
            arrowR.transform.localRotation = Quaternion.Euler(arrowAngle, 0, 0);
        }

        if (textField != null)
        {
            textField.color = textColor;
            textField.transform.localScale = new Vector3(textScale, textScale, textScale);
        }
    }

    Vector3 LerpByDistance(Vector3 a, Vector3 b, float x)
    {
        Vector3 p = a + x * (b - a);
        return p;
    }
}

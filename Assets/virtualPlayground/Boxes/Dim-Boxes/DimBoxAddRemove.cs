//test DimBox runtime behaviour
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimBoxes
{
    [ExecuteInEditMode]
    public class DimBoxAddRemove : MonoBehaviour
    {
        public string jsonData;

        // Start is called before the first frame update
        void Start()
        {
            DimBox dmb = GetComponent<DimBox>();
            if (dmb)
            {
                dmb.mApplied = false;
                jsonData = JsonUtility.ToJson(dmb);
            }
        }
        void OnMouseDown()
        {
            Debug.Log(gameObject.name);
            if (GetComponent<DimBox>())
            {
                Destroy(GetComponent<DimBox>());
            }
            else
            {
                DimBox dmb = gameObject.AddComponent<DimBox>();
                dmb.enabled = false;
                JsonUtility.FromJsonOverwrite(jsonData, dmb);
                dmb.enabled = true;
            }

        }

        // Update is called once per frame
        void Reset()
        {
            Debug.Log("Reset");
        }
    }
}
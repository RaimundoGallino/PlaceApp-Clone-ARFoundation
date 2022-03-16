using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string ItemName;
    public Sprite ItemImage;
    public string price;
    public string ItemDescription;
    public GameObject Item3DModel;
}

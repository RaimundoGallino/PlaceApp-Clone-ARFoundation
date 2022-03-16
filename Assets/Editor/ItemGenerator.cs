using UnityEngine;
using UnityEditor;

public class ItemGenerator : EditorWindow
{
    private string ItemName;
    private Sprite ItemImage;
    private string price;
    private string ItemDescription;
    private GameObject Item3DModel;
    
    [MenuItem("Tools/Item Generator")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<ItemGenerator>("Item Generator");
        window.minSize = new Vector2(400, 200);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Item Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space(20);

        EditorGUILayout.BeginVertical();

        EditorGUIUtility.labelWidth = 75;
        EditorGUIUtility.fieldWidth = position.width - 100;
        
        GUI.SetNextControlName("Item Name");
        
        ItemName = EditorGUILayout.TextField("Item Name", ItemName);
        EditorGUILayout.Space();
        ItemDescription = EditorGUILayout.TextField("Description", ItemDescription);
        EditorGUILayout.Space();
        price = EditorGUILayout.TextField("Price", price);
        EditorGUILayout.Space();
        ItemImage = (Sprite) EditorGUILayout.ObjectField("Item Image", ItemImage, typeof(Sprite));
        EditorGUILayout.Space();
        Item3DModel = (GameObject) EditorGUILayout.ObjectField("3D Model", Item3DModel, typeof(GameObject));
        
        EditorGUILayout.Space(40);

        if (GUILayout.Button("Generate Item"))
        {
            if (IsValidContent())
            {
                GenerateItem();
            }
        }

        EditorGUILayout.EndVertical();
    }

    private bool IsValidContent()
    {
        if (string.IsNullOrWhiteSpace(ItemName))
        {
            Debug.LogError("Item must have a name");
            return false;
        }
        if (string.IsNullOrWhiteSpace(ItemDescription))
        {
            Debug.LogWarning("Item don't have a description");
            return false;
        }
        if (string.IsNullOrWhiteSpace(price))
        {
            Debug.LogWarning("Item don't have a price");
            return false;
        }
        if (Item3DModel == null)
        {
            Debug.LogError("Item must have a prefab assigned");
            return false;
        }

        if (ItemImage == null)
        {
            Debug.LogError("Item must have an image assigned");
            return false;
        }

        return true;
    }

    private void GenerateItem()
    {
        Item item = CreateInstance<Item>();
        item.ItemName = ItemName;
        item.ItemDescription = ItemDescription;
        item.price = price;
        item.ItemImage = ItemImage;
        item.Item3DModel = Item3DModel;
    
        string itemPath = $"Assets/Scriptable Objects/{ItemName}.asset";

        string filename = AssetDatabase.GenerateUniqueAssetPath(itemPath);
        
        AssetDatabase.CreateAsset(item, filename);
        AssetDatabase.SaveAssets();
        
        ResetItem();
    }

    private void ResetItem()
    {
        ItemName = null;
        ItemDescription = null;
        price = null;
        ItemImage = null;
        Item3DModel = null;
        
        GUI.FocusControl("Item Name");
    }
}

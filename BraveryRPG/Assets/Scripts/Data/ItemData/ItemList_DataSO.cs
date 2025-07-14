using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item List", fileName = "List of Items - ")]
public class ItemList_DataSO : ScriptableObject
{
    public Item_DataSO[] itemList;

    // public Item_DataSO GetItemData(string saveId)
    // {
    //     return itemList.FirstOrDefault(item => item != null && item.saveId == saveId);
    // }

#if UNITY_EDITOR
    // [ContextMenu("Auto-fill with all Item_DataSO")]
    // public void CollectItemsData()
    // {
    //     string[] guids = AssetDatabase.FindAssets("t:Item_DataSO");

    //     itemList = guids
    //         .Select(guid => AssetDatabase.LoadAssetAtPath<Item_DataSO>(AssetDatabase.GUIDToAssetPath(guid)))
    //         .Where(item => item != null)
    //         .ToArray();

    //     EditorUtility.SetDirty(this);
    //     AssetDatabase.SaveAssets();
    // }
#endif
}

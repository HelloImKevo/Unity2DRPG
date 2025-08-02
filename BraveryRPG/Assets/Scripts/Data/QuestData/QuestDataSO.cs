using UnityEditor;
using UnityEngine;

public enum RewardType { Merchant, Blacksmith, None };

public enum QuestType { Kill, Talk, Delivery };

[CreateAssetMenu(menuName = "RPG Setup/Quest Data/New Quest", fileName = "Quest - ")]
public class QuestDataSO : ScriptableObject
{
    // NOTE: It's not a good practice to use get + private set on these properties,
    // because it can cause issues with Save System.
    public string questSaveId;

    [Space]

    public QuestType questType;
    public string questName;

    [TextArea] public string description;
    [TextArea] public string questGoal;

    public string questTargetId; // Enemy Name, NPC name, Item Name
    public int requiredAmount;
    public Item_DataSO itemToDeliver; // Used only if quest type is Delivery

    [Header("Reward")]
    public RewardType rewardProvider;
    public Inventory_Item[] rewardItems;

    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        questSaveId = AssetDatabase.AssetPathToGUID(path);
#endif
    }
}

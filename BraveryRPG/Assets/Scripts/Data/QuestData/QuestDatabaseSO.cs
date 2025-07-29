using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Quest Data/Quest Database", fileName = "QUEST DATABASE")]
public class QuestDatabaseSO : ScriptableObject
{
    public QuestDataSO[] allQuests;

    public QuestDataSO GetQuestById(string id)
    {
        return allQuests.FirstOrDefault(q => q != null && q.questSaveId == id);
    }

#if UNITY_EDITOR
    /// <summary>Mirrors logic from <see cref="ItemList_DataSO"/>.</summary>
    [ContextMenu("Auto-fill with all QuestDataSO")]
    public void CollectQuestData()
    {
        // This name must be an exact match to our QuestDataSO construct!
        // TODO: Could probably use QuestDataSO.GetType().Name
        string[] guids = AssetDatabase.FindAssets("t:QuestDataSO");

        allQuests = guids
             .Select(guid => AssetDatabase.LoadAssetAtPath<QuestDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
             .Where(q => q != null)
             .ToArray();

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}

using System.Collections.Generic;
using UnityEngine;

public class Player_QuestManager : MonoBehaviour, ISaveable
{
    public List<QuestData> activeQuests;
    public List<QuestData> completedQuests;

    private Entity_DropManager dropManager;
    private Inventory_Player inventory;

    [Header("QUEST DATABASE")]
    [SerializeField] private QuestDatabaseSO questDatabase;

    private void Awake()
    {
        dropManager = GetComponent<Entity_DropManager>();
        inventory = GetComponent<Inventory_Player>();
    }

    public void TryGiveRewardFrom(RewardType npcType)
    {
        List<QuestData> getRewardQuests = new();

        foreach (var quest in activeQuests)
        {
            // DELIVER ITEMS IF CAN
            if (QuestType.Delivery == quest.questDataSO.questType)
            {
                var requiredItem = quest.questDataSO.itemToDeliver;
                var requiredAmount = quest.questDataSO.requiredAmount;

                // if (inventory.HasItemAmount(requiredItem, requiredAmount))
                // {
                //     inventory.RemoveItemAmount(requiredItem, requiredAmount);
                //     quest.AddQuestProgress(requiredAmount);
                // }
            }

            // TODO: 'Talk to Blacksmith' quest is not getting completed. Need to revisit this.
            if (quest.CanGetReward() && quest.questDataSO.rewardType == npcType)
            {
                Debug.Log($"Player_QuestManager.TryGiveRewardFrom() -> '{quest.GetQuestName()}' {quest.GetQuestType()}" +
                          $" Quest Objectives Fulfilled - NPC Type = '{npcType}'");
                getRewardQuests.Add(quest);
            }
        }

        foreach (var quest in getRewardQuests)
        {
            DropQuestRewardsOnGround(quest.questDataSO);
            CompleteQuest(quest);
        }
    }

    public void TryAddProgress(string questTargetId, int amount = 1)
    {
        List<QuestData> getRewardQuests = new();

        foreach (var quest in activeQuests)
        {
            if (quest.questDataSO.questTargetId != questTargetId) continue;

            if (!quest.CanGetReward())
            {
                quest.AddQuestProgress(amount);
            }

            if (RewardType.None == quest.questDataSO.rewardType && quest.CanGetReward())
            {
                getRewardQuests.Add(quest);
            }
        }

        foreach (var quest in getRewardQuests)
        {
            DropQuestRewardsOnGround(quest.questDataSO);
            CompleteQuest(quest);
        }
    }

    private void DropQuestRewardsOnGround(QuestDataSO questDataSO)
    {
        foreach (var item in questDataSO.rewardItems)
        {
            if (item == null || item.itemData == null) continue;

            for (int i = 0; i < item.stackSize; i++)
            {
                dropManager.CreateItemDrop(item.itemData);
            }
        }
    }

    public int GetQuestProgress(QuestData questToCheck)
    {
        QuestData quest = activeQuests.Find(q => q == questToCheck);
        return quest != null ? quest.currentAmount : 0;
    }

    public void AcceptQuest(QuestDataSO questDataSO)
    {
        activeQuests.Add(new QuestData(questDataSO));
    }

    public void CompleteQuest(QuestData questData)
    {
        // TODO: Mark the QuestData as 'Completed' to avoid confusion with 'CanGetReward = true'
        completedQuests.Add(questData);
        activeQuests.Remove(questData);
    }

    public bool QuestIsActive(QuestDataSO questToCheck)
    {
        if (questToCheck == null)
        {
            return false;
        }

        return activeQuests.Find(q => q.questDataSO == questToCheck) != null;
    }

    #region Save System

    public void LoadData(GameData data)
    {
        activeQuests.Clear();

        foreach (var entry in data.activeQuests)
        {
            string questSaveId = entry.Key;
            int progress = entry.Value;

            QuestDataSO questDataSO = questDatabase.GetQuestById(questSaveId);

            if (questDataSO == null)
            {
                Debug.Log(questSaveId + " was not found in the database!");
                continue;
            }

            QuestData questToLoad = new(questDataSO);
            questToLoad.currentAmount = progress;

            activeQuests.Add(questToLoad);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.activeQuests.Clear();

        foreach (var quest in activeQuests)
        {
            data.activeQuests.Add(quest.questDataSO.questSaveId, quest.currentAmount);
        }

        foreach (var quest in completedQuests)
        {
            data.completedQuests.Add(quest.questDataSO.questSaveId, true);
        }
    }

    #endregion
}

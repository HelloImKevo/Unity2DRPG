using System;

[Serializable]
public class QuestData
{
    public QuestDataSO questDataSO;
    public int currentAmount;

    // TODO: Rename this to something like QuestRequirementsFulfilled, or
    // ObjectivesSatisfied. We also need to be able to RemoveQuestProgress,
    // for when the Player loses needed Quest items.
    public bool canGetReward;

    public void AddQuestProgress(int amount = 1)
    {
        currentAmount += amount;
        canGetReward = CanGetReward();
    }

    public bool CanGetReward() => currentAmount >= questDataSO.requiredAmount;

    public string GetQuestName() => questDataSO.questName;

    public QuestType GetQuestType() => questDataSO.questType;

    public QuestData(QuestDataSO questSO)
    {
        questDataSO = questSO;
    }
}

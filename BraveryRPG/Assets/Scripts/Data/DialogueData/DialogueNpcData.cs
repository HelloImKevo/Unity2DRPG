[System.Serializable]
public class DialogueNpcData
{
    public RewardType npcRewardType;
    public QuestDataSO[] quests;

    public DialogueNpcData(RewardType npcRewardType, QuestDataSO[] quests)
    {
        this.npcRewardType = npcRewardType;
        this.quests = quests;
    }
}

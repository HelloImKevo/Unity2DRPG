using TMPro;
using UnityEngine;

public class UI_ActiveQuestPreview : MonoBehaviour
{
    private Player_QuestManager questManager;

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI progress;

    [Space]
    [SerializeField] private UI_QuestRewardSlot[] questRewardSlots;

    public void SetupQuestPreview(QuestData questData)
    {
        if (questData == null)
        {
            // Clear all text fields.
            questName.text = "";
            description.text = "";
            progress.text = "";

            HideQuestRewardSlots();
            return;
        }

        questManager = Player.GetInstance().questManager;
        QuestDataSO questSO = questData.questDataSO;

        questName.text = questSO.name;
        description.text = questSO.description;

        // Eliminate Skeletons: 2/10
        progress.text = questSO.questGoal + " " + questManager.GetQuestProgress(questData) + "/" + questSO.requiredAmount;

        HideQuestRewardSlots();

        for (int i = 0; i < questSO.rewardItems.Length; i++)
        {
            questRewardSlots[i].gameObject.SetActive(true);
            questRewardSlots[i].UpdateSlot(questSO.rewardItems[i]);
        }
    }

    private void HideQuestRewardSlots()
    {
        foreach (var obj in questRewardSlots)
        {
            obj.gameObject.SetActive(false);
        }
    }
}

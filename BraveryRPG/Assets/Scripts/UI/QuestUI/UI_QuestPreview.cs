using TMPro;
using UnityEngine;

public class UI_QuestPreview : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private TextMeshProUGUI questGoal;
    [SerializeField] private UI_QuestRewardSlot[] questRewardSlots;

    [SerializeField] private GameObject[] additionalObjects;
    private UI_Quest questUI;
    private QuestDataSO previewQuest;

    public void SetupQuestPreview(QuestDataSO questDataSO)
    {
        Debug.Log($"UI_QuestPreview.SetupQuestPreview() -> Setting up quest: '{questDataSO.questName}'");

        questUI = transform.root.GetComponentInChildren<UI_Quest>();
        previewQuest = questDataSO;

        EnableAdditonalObjects(true);
        EnableQuestRewardObjects(false);

        questName.text = questDataSO.questName;
        questDescription.text = questDataSO.description;
        questGoal.text = questDataSO.questGoal + " " + questDataSO.requiredAmount;

        for (int i = 0; i < questDataSO.rewardItems.Length; i++)
        {
            Inventory_Item rewardItem = new(questDataSO.rewardItems[i].itemData);
            rewardItem.stackSize = questDataSO.rewardItems[i].stackSize;

            questRewardSlots[i].gameObject.SetActive(true);
            questRewardSlots[i].UpdateSlot(rewardItem);
        }
    }

    public void AcceptQuestBTN()
    {
        MakeQuestPreviewEmpty();

        // questUI.questManager.AcceptQuest(previewQuest);
        questUI.UpdateQuestList();
    }

    public void MakeQuestPreviewEmpty()
    {
        questName.text = "";
        questDescription.text = "";

        EnableAdditonalObjects(false);
        EnableQuestRewardObjects(false);
    }

    private void EnableAdditonalObjects(bool enable)
    {
        foreach (var obj in additionalObjects)
        {
            obj.SetActive(enable);
        }
    }

    private void EnableQuestRewardObjects(bool enable)
    {
        foreach (var obj in questRewardSlots)
        {
            obj.gameObject.SetActive(enable);
        }
    }
}

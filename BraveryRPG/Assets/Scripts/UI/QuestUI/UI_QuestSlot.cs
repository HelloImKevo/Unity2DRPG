using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private Image[] rewardQuickPreviewSlots;

    public QuestDataSO questInSlot { get; private set; }
    private UI_QuestPreview questPreview;

    public void SetupQuestSlot(QuestDataSO questDataSO)
    {
        questPreview = transform.root.GetComponentInChildren<UI_Quest>().GetQuestPreviewUI();

        questInSlot = questDataSO;
        questName.text = questDataSO.questName;

        foreach (var previewIcon in rewardQuickPreviewSlots)
        {
            previewIcon.gameObject.SetActive(false);
        }

        for (int i = 0; i < questInSlot.rewardItems.Length; i++)
        {
            var rewardItem = questDataSO.rewardItems[i];

            if (rewardItem == null || rewardItem.itemData == null) continue;

            Image slot = rewardQuickPreviewSlots[i];

            slot.gameObject.SetActive(true);
            slot.sprite = rewardItem.itemData.itemIcon;
            slot.GetComponentInChildren<TextMeshProUGUI>().text = rewardItem.stackSize.ToString();
        }
    }

    /// <summary>Invoked by the UI_QuestListBTN Button OnClick() event.</summary>
    public void UpdateQuestPreview()
    {
        questPreview.SetupQuestPreview(questInSlot);
    }
}

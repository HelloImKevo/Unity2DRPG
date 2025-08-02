using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ActiveQuestSlot : MonoBehaviour
{
    private QuestData questInSlot;
    private UI_ActiveQuestPreview questPreview;

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private Image[] questRewardPreview;

    public void SetupActiveQuestSlot(QuestData questToSetup)
    {
        questPreview = transform.root.GetComponentInChildren<UI_ActiveQuestPreview>();
        questInSlot = questToSetup;

        questName.text = questToSetup.questDataSO.questName;

        Inventory_Item[] rewardItems = questToSetup.questDataSO.rewardItems;

        foreach (var previewIcon in questRewardPreview)
        {
            previewIcon.gameObject.SetActive(false);
        }

        for (int i = 0; i < rewardItems.Length; i++)
        {
            if (rewardItems[i] == null) continue;

            Image preview = questRewardPreview[i];

            preview.gameObject.SetActive(true);
            preview.sprite = rewardItems[i].itemData.itemIcon;
            preview.GetComponentInChildren<TextMeshProUGUI>().text = rewardItems[i].stackSize.ToString();
        }
    }

    public void SetupPreviewBTN()
    {
        questPreview.SetupQuestPreview(questInSlot);
    }
}

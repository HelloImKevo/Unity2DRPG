using UnityEngine;

public class UI_Quest : MonoBehaviour, ISaveable
{
    private GameData currentGameData;

    [SerializeField] private UI_ItemSlotParent inventorySlots;
    [SerializeField] private UI_QuestPreview questPreview;

    private UI_QuestSlot[] questSlots;
    public Player_QuestManager questManager { get; private set; }

    private void Awake()
    {
        questSlots = GetComponentsInChildren<UI_QuestSlot>(true);
        questManager = Player.GetInstance().questManager;
    }

    public void SetupQuestUI(QuestDataSO[] questsToSetup)
    {
        foreach (var slot in questSlots)
        {
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < questsToSetup.Length; i++)
        {
            // TODO: This requires a lot of GameObjects to be pre-created in the Unity Editor,
            // and you'd have to add 99 list items, otherwise we'll get an IOOB Exception.
            // It would be more scalable to instantiate items as children. For huge quest
            // lists, you'd probably want to paginate the results to avoid a long scrollview.
            questSlots[i].gameObject.SetActive(true);
            questSlots[i].SetupQuestSlot(questsToSetup[i]);
        }

        questPreview.MakeQuestPreviewEmpty();
        inventorySlots.UpdateSlots(Player.GetInstance().Inventory.itemList);

        UpdateQuestList();
    }

    public void UpdateQuestList()
    {
        foreach (var slot in questSlots)
        {
            if (slot.questInSlot == null) continue;

            if (slot.gameObject.activeSelf && !CanTakeQuest(slot.questInSlot))
            {
                slot.gameObject.SetActive(false);
            }
        }
    }

    private bool CanTakeQuest(QuestDataSO questToCheck)
    {
        bool questActive = questManager.QuestIsActive(questToCheck);

        // if (currentGameData != null)
        // {
        //     bool questCompleted =
        //         currentGameData.completedQuests.TryGetValue(questToCheck.questSaveId, out bool isCompleted) && isCompleted;

        //     return questActive == false && questCompleted == false;
        // }

        return !questActive;
    }

    public UI_QuestPreview GetQuestPreview() => questPreview;

    #region Save System

    public void LoadData(GameData data)
    {
        currentGameData = data;
    }

    public void SaveData(ref GameData data)
    {
    }

    #endregion
}

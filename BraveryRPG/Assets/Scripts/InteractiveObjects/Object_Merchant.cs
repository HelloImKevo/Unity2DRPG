using UnityEngine;
using UnityEngine.Localization;

public class Object_Merchant : Object_NPC, IInteractable
{
    [Space]
    [Header("Quest & Dialogue")]
    [SerializeField] private DialogueLineSO firstDialogueLine;
    [SerializeField] private QuestDataSO[] quests;

    [Header("Localized Text")]
    [SerializeField] private LocalizedString welcomeText;

    private Inventory_Player inventory;
    private Inventory_Merchant merchant;

    protected override void Awake()
    {
        base.Awake();
        merchant = GetComponent<Inventory_Merchant>();
    }

    protected override void Update()
    {
        base.Update();

        // DEBUGGING: Allow player to press 'Z' to refresh for sale items.
        if (Input.GetKeyDown(KeyCode.Z))
        {
            merchant.FillShopList();
        }
    }

    public override void Interact()
    {
        base.Interact();

        if (merchant == null || inventory == null)
        {
            Debug.LogWarning($"{gameObject.name}.Interact() -> merchant or inventory" +
                              " is null - Check Layer Collision Map for Player+NPC");
        }

        ui.OpenDialogueUI(firstDialogueLine, new DialogueNpcData(rewardNpc, quests));
        // ui.OpenQuestUI(quests);

        // TODO: Temporarily switch from Merchant Behavior to Quest-Giver mode.
        if (true) return;

        ui.merchantUI.SetupMerchantUI(merchant, inventory);
        ui.OpenMerchantUI(true);

        ToastStyle style = new()
        {
            textColor = Color.white,
            backgroundColor = new Color(0.2f, 0, 0),
            blinkColor = Color.white,
            enableBlink = false,
            duration = 3f
        };

        // Welcome to the Merchant Shop!
        string message = welcomeText.GetLocalizedString();
        ToastManager.Instance.ShowToast(message, style, ToastAnchor.BottomCenter);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        inventory = player.GetComponent<Inventory_Player>();
        merchant.SetInventory(inventory);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        ui.HideAllTooltips();
        ui.OpenMerchantUI(false);
    }
}

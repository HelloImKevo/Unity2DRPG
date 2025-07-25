using UnityEngine;

public class Object_Merchant : Object_NPC, IInteractable
{
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

    public void Interact()
    {
        if (merchant == null || inventory == null)
        {
            Debug.LogWarning($"{gameObject.name}.Interact() -> merchant or inventory" +
                              " is null - Check Layer Collision Map for Player+NPC");
        }

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
        ToastManager.Instance.ShowToast("Welcome to the Merchant Shop!", style, ToastAnchor.BottomCenter);
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

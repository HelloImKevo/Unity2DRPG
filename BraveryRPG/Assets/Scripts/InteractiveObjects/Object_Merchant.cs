using UnityEngine;

public class Object_Merchant : Object_NPC, IInteractable
{
    private Inventory_Player inventory;
    // private Inventory_Merchant merchant;

    protected override void Awake()
    {
        base.Awake();
        // merchant = GetComponent<Inventory_Merchant>();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            // merchant.FillShopList();

            // Demonstrate Toast Usage:
            ToastStyle toastStyle = new()
            {
                textColor = Color.white,
                backgroundColor = new Color(0.2f, 0, 0),
                blinkColor = Color.seaGreen,
                enableBlink = true,
                duration = 2f
            };
            ToastManager.Instance.ShowToast("Welcome to my Shop!", toastStyle, ToastAnchor.BottomCenter);
        }
    }

    public void Interact()
    {
        // ui.merchantUI.SetupMerchantUI(merchant, inventory);
        // ui.OpenMerchantUI(true);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        inventory = player.GetComponent<Inventory_Player>();
        // merchant.SetInventory(inventory);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        // ui.HideAllTooltips();
        // ui.OpenMerchantUI(false);
    }
}

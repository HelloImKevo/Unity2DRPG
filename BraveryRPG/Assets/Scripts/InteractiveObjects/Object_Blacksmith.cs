using UnityEngine;
using UnityEngine.Localization;

/// <summary>
/// Must have the <see cref="Inventory_Storage"/> script component!
/// </summary>
public class Object_Blacksmith : Object_NPC, IInteractable
{
    [Header("Localized Text")]
    [SerializeField] private LocalizedString welcomeText;

    private Animator anim;
    private Inventory_Player inventory;
    private Inventory_Storage storage;

    protected override void Awake()
    {
        base.Awake();
        storage = GetComponent<Inventory_Storage>();

        if (storage == null)
        {
            Debug.LogWarning($"{GetType().Name} Inventory_Storage component is null, did you forget to add it as a component?");
        }

        anim = GetComponentInChildren<Animator>();
        // The Merchant and Blacksmith are both using the same NPC
        // Animator Controller - this will activate the Blacksmith
        // variant of the shared spritesheet.
        anim.SetBool("isBlacksmith", true);
    }

    public override void Interact()
    {
        base.Interact();

        ui.storageUI.SetupStorageUI(storage);
        ui.craftUI.SetupCraftUI(storage);

        ui.OpenStorageUI(true);

        // Demonstrate Toast Usage:
        ToastStyle toastStyle = new()
        {
            textColor = Color.white,
            backgroundColor = new Color(0.2f, 0, 0),
            blinkColor = Color.seaGreen,
            enableBlink = true,
            duration = 3f
        };

        // NOTE: After adding the Localization package, VS Code would not recognize
        // the `using` directive until I restarted VS Code and Unity.

        // Retrieve the localized string synchronously
        // Welcome to the Blacksmith Storage & Crafting!
        string message = welcomeText.GetLocalizedString();
        ToastManager.Instance.ShowToast(message, toastStyle, ToastAnchor.BottomCenter);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        inventory = player.GetComponent<Inventory_Player>();
        storage.SetInventory(inventory);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        ui.HideAllTooltips();
        ui.OpenStorageUI(false);
    }
}

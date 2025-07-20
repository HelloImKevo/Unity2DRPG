using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Inventory_Item itemInSlot { get; private set; }

    protected Inventory_Player inventory;
    protected UI ui;
    protected RectTransform rect;

    [Header("UI Slot Setup")]
    [SerializeField] protected Image itemIcon;
    [SerializeField] protected TextMeshProUGUI itemStackSizeLabel;

    protected virtual void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        inventory = FindAnyObjectByType<Inventory_Player>();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null || ItemType.Material == itemInSlot.itemData.itemType)
        {
            // You cannot equip a Crafting Material!
            return;
        }

        bool alternativeInput = Input.GetKey(KeyCode.LeftControl);

        if (alternativeInput)
        {
            inventory.RemoveOneItem(itemInSlot);
        }
        else
        {
            if (ItemType.Consumable == itemInSlot.itemData.itemType)
            {
                inventory.TryUseItem(itemInSlot);
            }
            else
            {
                // Equip the item to the player equipment list.
                inventory.TryEquipItem(itemInSlot);
            }
        }

        if (itemInSlot == null)
        {
            ui.itemTooltip.HideTooltip();
        }
    }

    public void UpdateSlot(Inventory_Item item)
    {
        itemInSlot = item;

        if (itemInSlot == null)
        {
            itemStackSizeLabel.text = "";
            itemIcon.color = Color.clear;
            return;
        }

        // Make the icon slightly transparent.
        Color color = Color.white;
        color.a = 0.9f;
        itemIcon.color = color;

        itemIcon.sprite = itemInSlot.itemData.itemIcon;
        itemStackSizeLabel.text = item.stackSize > 1 ? item.stackSize.ToString() : "";
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null) return;

        ui.itemTooltip.ShowTooltip(true, rect, itemInSlot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.itemTooltip.HideTooltip();
    }
}

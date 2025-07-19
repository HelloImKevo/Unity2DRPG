using System.Collections.Generic;
using UnityEngine;

public class Player_DropManager : Entity_DropManager
{
    [Header("Player Drop Details")]
    [Range(0, 100)]
    [Tooltip("Percent chance for each item to be dropped from the player's inventory upon death.")]
    [SerializeField] private float chanceToLoseItem = 90f;

    private Inventory_Player inventory;

    private void Awake()
    {
        inventory = GetComponent<Inventory_Player>();
    }

    public override void DropItems()
    {
        if (chanceToLoseItem <= 0) return;

        // Make a copy of the player's inventory, to avoid NullReferenceException.
        List<Inventory_Item> inventoryItemsCopy = new(inventory.itemList);
        List<Inventory_EquipmentSlot> equipmentSlotsCopy = new(inventory.equipmentList);

        foreach (var item in inventoryItemsCopy)
        {
            if (Random.Range(0, 100) < chanceToLoseItem)
            {
                CreateItemDrop(item.itemData);
                inventory.RemoveFullStack(item);
            }
        }

        foreach (var equip in equipmentSlotsCopy)
        {
            if (Random.Range(0, 100) < chanceToLoseItem && equip.HasItem())
            {
                var item = equip.GetEquippedItem();

                CreateItemDrop(item.itemData);
                inventory.UnequipItem(item);
                inventory.RemoveFullStack(item);
            }
        }
    }
}

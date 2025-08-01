using System.Collections.Generic;
using UnityEngine;

public class UI_ItemSlotParent : MonoBehaviour
{
    private UI_ItemSlot[] slots;

    public void UpdateSlots(List<Inventory_Item> itemList)
    {
        if (slots == null)
        {
            slots = GetComponentsInChildren<UI_ItemSlot>();
        }

        // The number of items in our inventory can be less than the number
        // of UI slots in our Inventory UI.
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < itemList.Count)
            {
                // Update the slot with the contents of our item data.
                slots[i].UpdateSlot(itemList[i]);
            }
            else
            {
                // Empty item slot.
                slots[i].UpdateSlot(null);
            }
        }
    }
}

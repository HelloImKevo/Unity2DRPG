using UnityEngine;

public class UI_PlayerStats : MonoBehaviour
{
    private UI_StatSlot[] uiStatSlots;
    private Inventory_Player inventory;

    private void Awake()
    {
        // Collect all child UI_StatSlot components within the widget hierarchy.
        uiStatSlots = GetComponentsInChildren<UI_StatSlot>();

        inventory = FindFirstObjectByType<Inventory_Player>();
        // Subscribe to event action and refresh our Player Stats UI
        // whenever the inventory contents are changed.
        inventory.OnInventoryChange += UpdateStatsUI;
    }

    private void Start()
    {
        UpdateStatsUI();
    }

    /// <summary>
    /// Traverse all Stat Slots and update its displayed Calculated Stat Value.
    /// </summary>
    private void UpdateStatsUI()
    {
        foreach (var statSlot in uiStatSlots)
        {
            statSlot.UpdateStatValue();
        }
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private Player player;
    private Inventory_Player inventory;
    private UI_SkillSlot[] skillSlots;

    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Quick Item Slots")]
    [SerializeField] private float yOffsetQuickItemParent = 150f;
    [SerializeField] private Transform quickItemOptionsParent;
    private UI_QuickItemSlotOption[] quickItemOptions;
    private UI_QuickItemSlot[] quickItemSlots;

    private void Start()
    {
        quickItemSlots = GetComponentsInChildren<UI_QuickItemSlot>();

        player = FindFirstObjectByType<Player>();
        player.Health.OnHealthUpdate += UpdateHealthBar;

        inventory = player.Inventory;
        inventory.OnInventoryChange += UpdateQuickSlotsUI;
        inventory.OnQuickSlotUsed += PlayQuickSlotFeedback;
    }

    public void PlayQuickSlotFeedback(int slotNumber) => quickItemSlots[slotNumber].SimulateButtonFeedback();

    public void UpdateQuickSlotsUI()
    {
        Inventory_Item[] quickItems = inventory.quickItems;

        for (int i = 0; i < quickItems.Length; i++)
        {
            if (i >= quickItemSlots.Length) break;

            quickItemSlots[i].UpdateQuickSlotUI(quickItems[i]);
        }
    }

    /// <summary>
    /// Display the floating options which depict items that can be assigned to the Quick Item slot.
    /// </summary>
    public void OpenQuickItemOptions(UI_QuickItemSlot quickItemSlot, RectTransform targetRect)
    {
        if (quickItemOptions == null)
        {
            quickItemOptions = quickItemOptionsParent.GetComponentsInChildren<UI_QuickItemSlotOption>(true);
        }

        List<Inventory_Item> consumables = inventory.itemList.FindAll(
            item => item.itemData.itemType == ItemType.Consumable
        );

        for (int i = 0; i < quickItemOptions.Length; i++)
        {
            if (i < consumables.Count)
            {
                quickItemOptions[i].gameObject.SetActive(true);
                quickItemOptions[i].SetupOption(quickItemSlot, consumables[i]);
            }
            else
            {
                quickItemOptions[i].gameObject.SetActive(false);
            }
        }

        // Display available assignment options above the Quick Item slot.
        quickItemOptionsParent.position = targetRect.position + Vector3.up * yOffsetQuickItemParent;
    }

    public void HideQuickItemOptions() => quickItemOptionsParent.position = new Vector3(0, 9999);

    public UI_SkillSlot GetSkillSlot(SkillType skillType)
    {
        if (skillSlots == null)
        {
            skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);
        }

        foreach (var slot in skillSlots)
        {
            if (slot.skillType == skillType)
            {
                slot.gameObject.SetActive(true);
                return slot;
            }
        }

        return null;
    }

    private void UpdateHealthBar()
    {
        float currentHealth = Mathf.RoundToInt(player.Health.GetCurrentHealth());
        float maxHealth = player.Stats.GetMaxHealth();

        // Increase the horizontal width of the health bar, like Dead Cells and
        // other similar games.
        float sizeDifference = Mathf.Abs(maxHealth - healthRect.sizeDelta.x);
        if (sizeDifference > 0.1f)
        {
            // If your game has huge health pools, like 12,000 HP,
            // multiply the maxHealth var by a multiplier, like 0.25f
            healthRect.sizeDelta = new Vector2(maxHealth, healthRect.sizeDelta.y);
        }

        healthText.text = currentHealth + "/" + maxHealth;
        healthSlider.value = player.Health.GetHealthPercent();
    }
}

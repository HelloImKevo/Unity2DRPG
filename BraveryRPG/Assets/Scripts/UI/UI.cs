using UnityEngine;

/// <summary>
/// Main UI controller that manages references to various UI components and tooltips.
/// Acts as a central hub for UI element coordination.
/// </summary>
public class UI : MonoBehaviour
{
    // User Interface View Controllers
    public UI_SkillTree skillTreeUI { get; private set; }
    public UI_Inventory inventoryUI { get; private set; }

    // Tooltips
    public UI_SkillTooltip skillTooltip { get; private set; }
    public UI_ItemTooltip itemTooltip { get; private set; }
    public UI_StatTooltip statTooltip { get; private set; }

    private bool skillTreeEnabled;
    private bool inventoryEnabled;

    /// <summary>
    /// Initializes UI component references on awake.
    /// </summary>
    void Awake()
    {
        skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);
        inventoryUI = GetComponentInChildren<UI_Inventory>(true);

        skillTooltip = GetComponentInChildren<UI_SkillTooltip>(true);
        itemTooltip = GetComponentInChildren<UI_ItemTooltip>(true);
        statTooltip = GetComponentInChildren<UI_StatTooltip>(true);

        if (skillTreeUI == null)
        {
            Debug.LogWarning("Skill Tree component is null, did you forget to assign it to the UI script?");
        }

        if (inventoryUI == null)
        {
            Debug.LogWarning("Inventory component is null, did you forget to assign it to the UI script?");
        }

        if (skillTooltip == null)
        {
            Debug.LogWarning("Skill Tooltip component is null, did you forget to assign it to the UI script?");
        }

        if (itemTooltip == null)
        {
            Debug.LogWarning("Item Tooltip component is null, did you forget to assign it to the UI script?");
        }

        if (statTooltip == null)
        {
            Debug.LogWarning("Stat Tooltip component is null, did you forget to assign it to the UI script?");
        }

        // Fix bug where you have to press the UI input keybinding twice, in the
        // scenario where the UI component is already visible and active.
        skillTreeEnabled = skillTreeUI.gameObject.activeSelf;
        inventoryEnabled = inventoryUI.gameObject.activeSelf;
    }

    public void ToggleSkillTreeUI()
    {
        skillTreeEnabled = !skillTreeEnabled;
        // Activate / Deactivate game objects.
        skillTreeUI.gameObject.SetActive(skillTreeEnabled);
        // Hide the tooltip (move it into outer space).
        skillTooltip.HideTooltip();
    }

    public void ToggleInventoryUI()
    {
        inventoryEnabled = !inventoryEnabled;
        // Activate / Deactivate game objects.
        inventoryUI.gameObject.SetActive(inventoryEnabled);
        // Hide the tooltips (move them into outer space).
        itemTooltip.HideTooltip();
        statTooltip.HideTooltip();
    }
}

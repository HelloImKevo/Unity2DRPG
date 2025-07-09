using UnityEngine;

/// <summary>
/// Main UI controller that manages references to various UI components and tooltips.
/// Acts as a central hub for UI element coordination.
/// </summary>
public class UI : MonoBehaviour
{
    public UI_SkillTree skillTreeUI;
    public UI_SkillTooltip skillTooltip;

    public UI_ItemTooltip itemTooltip;
    public UI_StatTooltip statTooltip;

    // public UI_Inventory inventoryUI;

    private bool skillTreeEnabled;

    /// <summary>
    /// Initializes UI component references on awake.
    /// </summary>
    void Awake()
    {
        skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);
        skillTooltip = GetComponentInChildren<UI_SkillTooltip>(true);

        itemTooltip = GetComponentInChildren<UI_ItemTooltip>(true);
        statTooltip = GetComponentInChildren<UI_StatTooltip>(true);

        if (skillTreeUI == null)
        {
            Debug.LogWarning("Skill Tree component is null, did you forget to assign it to the UI script?");
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
    }

    public void ToggleSkillTreeUI()
    {
        skillTreeEnabled = !skillTreeEnabled;
        // Activate / Deactivate game objects.
        skillTreeUI.gameObject.SetActive(skillTreeEnabled);
        // Hide the tooltip (move it into outer space).
        skillTooltip.HideTooltip();
    }
}

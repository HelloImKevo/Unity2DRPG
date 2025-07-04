using UnityEngine;

/// <summary>
/// Main UI controller that manages references to various UI components and tooltips.
/// Acts as a central hub for UI element coordination.
/// </summary>
public class UI : MonoBehaviour
{
    public UI_SkillTree skillTree;
    public UI_SkillTooltip skillTooltip;

    private bool skillTreeEnabled;

    /// <summary>
    /// Initializes UI component references on awake.
    /// </summary>
    void Awake()
    {
        skillTree = GetComponentInChildren<UI_SkillTree>(true);
        skillTooltip = GetComponentInChildren<UI_SkillTooltip>(true);

        if (skillTree == null)
        {
            Debug.LogWarning("Skill Tree component is null, did you forget to assign it to the UI script?");
        }

        if (skillTooltip == null)
        {
            Debug.LogWarning("Skill Tooltip component is null, did you forget to assign it to the UI script?");
        }
    }

    public void ToggleSkillTreeUI()
    {
        skillTreeEnabled = !skillTreeEnabled;
        // Activate / Deactivate game objects.
        skillTree.gameObject.SetActive(skillTreeEnabled);
        // Hide the tooltip (move it into outer space).
        skillTooltip.ShowTooltip(false, null);
    }
}

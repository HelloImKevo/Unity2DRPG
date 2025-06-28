using UnityEngine;

/// <summary>
/// Main UI controller that manages references to various UI components and tooltips.
/// Acts as a central hub for UI element coordination.
/// </summary>
public class UI : MonoBehaviour
{
    public UI_SkillTooltip skillTooltip;
    public UI_SkillTree skillTree;
    private bool skillTreeEnabled;

    /// <summary>
    /// Initializes UI component references on awake.
    /// </summary>
    void Awake()
    {
        skillTooltip = GetComponentInChildren<UI_SkillTooltip>();
        skillTree = GetComponentInChildren<UI_SkillTree>(true);
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

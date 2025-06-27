using UnityEngine;

/// <summary>
/// Main UI controller that manages references to various UI components and tooltips.
/// Acts as a central hub for UI element coordination.
/// </summary>
public class UI : MonoBehaviour
{
    public UI_SkillTooltip skillTooltip;

    /// <summary>
    /// Initializes UI component references on awake.
    /// </summary>
    void Awake()
    {
        skillTooltip = GetComponentInChildren<UI_SkillTooltip>();
    }
}

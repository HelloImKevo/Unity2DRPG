using TMPro;
using UnityEngine;

public class UI_SkillTooltip : UI_Tooltip
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillRequirements;

    public override void ShowTooltip(bool show, RectTransform targetRect)
    {
        base.ShowTooltip(show, targetRect);
    }

    /// <summary>
    /// Overload that accepts a Skill Data ScriptableObject instance to populate
    /// the tooltip with skill details.
    /// </summary>
    /// <param name="show"></param>
    /// <param name="targetRect"></param>
    /// <param name="skillData"></param>
    public void ShowTooltip(bool show, RectTransform targetRect, Skill_DataSO skillData)
    {
        base.ShowTooltip(show, targetRect);

        if (!show) return;

        skillName.text = skillData.displayName;
        skillDescription.text = skillData.description;
        skillRequirements.text = "Requirements: \n"
            + " - " + skillData.cost + " skill point.";
    }
}

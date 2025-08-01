using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the skill tooltip display, showing skill information, requirements, and unlock conditions.
/// Provides dynamic content updates based on skill node states and player progress.
/// </summary>
public class UI_SkillTooltip : UI_Tooltip
{
    private UI ui;
    private UI_SkillTree skillTree;

    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillCooldown;
    [SerializeField] private TextMeshProUGUI skillRequirements;

    [Space]
    [Tooltip("Hex value to colorize text for prerequisite conditions that HAVE been met.")]
    [SerializeField] private string hexConditionMet;

    [Tooltip("Hex value to colorize text for prerequisite conditions that have NOT been met.")]
    [SerializeField] private string hexConditionNotMet;

    [Tooltip("Hex value to colorize text for important info, such as other skills that will be locked out.")]
    [SerializeField] private string hexImportantInfo;
    [SerializeField] private Color colorPickerHelper = Color.yellow;
    [SerializeField] private string lockedSkillText = "You've taken a different path - this skill is now locked.";

    private Coroutine textEffectCo;

    /// <summary>
    /// Initializes component references and calls base awake functionality.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        ui = GetComponentInParent<UI>();
        skillTree = ui.GetComponentInChildren<UI_SkillTree>(true);
    }

    /// <summary>
    /// Overload that accepts a [UI_TreeNode] containing a Skill Data ScriptableObject
    // instance to populate the tooltip with skill details.
    /// </summary>
    /// <param name="show"></param>
    /// <param name="targetRect"></param>
    /// <param name="node"></param>
    public void ShowTooltip(bool show, RectTransform targetRect, Skill_DataSO skillData, UI_TreeNode node)
    {
        base.ShowTooltip(show, targetRect);

        if (!show) return;

        skillName.text = skillData.displayName;
        skillDescription.text = skillData.description;
        skillCooldown.text = "Cooldown " + skillData.upgradeData.cooldown + " s.";

        if (node == null)
        {
            skillRequirements.text = "";
            return;
        }

        string skillLockedText = GetColoredText(hexImportantInfo, lockedSkillText);
        string requirements = node.isLocked
            ? skillLockedText
            : GetRequirements(node.skillData.cost, node.requiredNodes, node.conflictNodes);
        skillRequirements.text = requirements;
    }

    public void PlayLockedSkillBlinkEffect()
    {
        if (textEffectCo != null) StopCoroutine(textEffectCo);

        textEffectCo = StartCoroutine(TextBlinkEffectCo(skillRequirements, 0.15f, 3));
    }

    public void StopLockedSkillBlinkEffect()
    {
        if (textEffectCo != null)
        {
            StopCoroutine(textEffectCo);
        }
    }

    private IEnumerator TextBlinkEffectCo(TextMeshProUGUI text, float blinkInterval, int blinkCount)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            text.text = GetColoredText(hexConditionNotMet, lockedSkillText);
            yield return new WaitForSeconds(blinkInterval);

            text.text = GetColoredText(hexImportantInfo, lockedSkillText);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    /// <summary>
    /// Builds and formats the requirements text for a skill, including cost, prerequisites, and conflicts.
    /// </summary>
    /// <param name="skillCost">The skill point cost required to unlock the skill.</param>
    /// <param name="requiredNodes">Array of skill nodes that must be unlocked first.</param>
    /// <param name="conflictNodes">Array of skill nodes that will be locked out if this skill is taken.</param>
    /// <returns>Formatted requirements text with appropriate color coding.</returns>
    private string GetRequirements(int skillCost, UI_TreeNode[] requiredNodes, UI_TreeNode[] conflictNodes)
    {
        StringBuilder sb = new();

        // TODO: Hide the requirements once the skill is unlocked.
        sb.AppendLine("Requirements:");

        string costColor = skillTree.EnoughSkillPoints(skillCost) ? hexConditionMet : hexConditionNotMet;
        string costText = $"- {skillCost} skill point(s)";
        string finalCostText = GetColoredText(costColor, costText);

        sb.AppendLine(finalCostText);

        foreach (var requiredNode in requiredNodes)
        {
            if (requiredNode == null) continue;

            string nodeColor = requiredNode.isUnlocked ? hexConditionMet : hexConditionNotMet;
            string nodeText = $"- {requiredNode.skillData.displayName}";
            string finalNodeText = GetColoredText(nodeColor, nodeText);

            sb.AppendLine(finalNodeText);
        }

        if (conflictNodes.Length <= 0)
        {
            return sb.ToString();
        }

        sb.AppendLine(); // Spacing
        sb.AppendLine(GetColoredText(hexImportantInfo, "Locks out: "));

        foreach (var conflictNode in conflictNodes)
        {
            if (conflictNode == null) continue;

            string nodeText = $"- {conflictNode.skillData.displayName}";
            string finalNodeText = GetColoredText(hexImportantInfo, nodeText);
            sb.AppendLine(finalNodeText);
        }

        return sb.ToString();
    }
}

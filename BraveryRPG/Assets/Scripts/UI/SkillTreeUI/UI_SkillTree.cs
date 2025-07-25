using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the skill tree system including skill point allocation, validation, and tree operations.
/// Controls the overall state and behavior of the skill progression system.
/// </summary>
public class UI_SkillTree : MonoBehaviour, ISaveable
{
    [SerializeField] private int skillPoints;

    [Tooltip("TextView that displays the number of Skill Points available to the Player.")]
    [SerializeField] private TextMeshProUGUI skillPointsText;
    [Tooltip("The top-most node in the tree hierarchy. Required for the UpdateAllConnections context menu action.")]
    [SerializeField] private UI_TreeConnectHandler[] parentNodes;

    public Player_SkillManager SkillManager { get; private set; }

    private UI_TreeNode[] allTreeNodes;

    /// <summary>Initializes the skill tree on start.</summary>
    private void Start()
    {
        UpdateAllConnections();
        UpdateSkillPointsUI();
    }

    private void UpdateSkillPointsUI()
    {
        skillPointsText.text = skillPoints.ToString();
    }

    public void UnlockDefaultSkills()
    {
        SkillManager = FindAnyObjectByType<Player_SkillManager>();
        allTreeNodes = GetComponentsInChildren<UI_TreeNode>(true);

        foreach (var node in allTreeNodes)
        {
            node.UnlockDefaultSkill();
        }
    }

    /// <summary>
    /// Resets all skills in the tree, refunding spent skill points and unlocking all nodes.
    /// </summary>
    [ContextMenu("Reset Skill Tree")]
    public void RefundAllSkills()
    {
        UI_TreeNode[] skillNodes = GetComponentsInChildren<UI_TreeNode>();

        foreach (var node in skillNodes)
        {
            node.Refund();
        }

        // TODO: This does not account for 'Unlocked By Default' skills. We could potentially pass
        // in an Array. But it would probably be better for the SkillManager to know which Skills are
        // unlocked by default, rather than the UI managing this behavior.
        SkillManager.ResetAllSkills();
    }

    /// <summary>
    /// Checks if the player has enough skill points to afford a skill.
    /// </summary>
    /// <param name="cost">The skill point cost to check against.</param>
    /// <returns>True if the player has enough skill points, false otherwise.</returns>
    public bool EnoughSkillPoints(int cost) => skillPoints >= cost;

    /// <summary>
    /// Removes skill points from the player's available pool.
    /// </summary>
    /// <param name="cost">The number of skill points to remove.</param>
    public void RemoveSkillPoints(int cost)
    {
        skillPoints -= cost;
        UpdateSkillPointsUI();
    }

    /// <summary>
    /// Adds skill points to the player's available pool.
    /// </summary>
    /// <param name="points">The number of skill points to add.</param>
    public void AddSkillPoints(int points)
    {
        skillPoints += points;
        UpdateSkillPointsUI();
    }

    /// <summary>
    /// Use to rebuild the positions and sorting of all nodes in the skill tree.
    /// </summary>
    [ContextMenu("Update All Connections")]
    public void UpdateAllConnections()
    {
        foreach (var node in parentNodes)
        {
            node.UpdateAllConnections();
        }
    }

    #region ISaveable

    public void SaveData(ref GameData data)
    {
        data.skillPoints = skillPoints;
        data.skillTreeUI.Clear();
        data.skillUpgrades.Clear();

        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillData.displayName;
            data.skillTreeUI[skillName] = node.isUnlocked;
        }

        foreach (var skill in SkillManager.allSkills)
        {
            data.skillUpgrades[skill.GetSkillType()] = skill.GetUpgrade();
        }
    }

    public void LoadData(GameData data)
    {
        skillPoints = data.skillPoints;

        foreach (var node in allTreeNodes)
        {
            string skillName = node.skillData.displayName;

            if (data.skillTreeUI.TryGetValue(skillName, out bool unlocked) && unlocked)
            {
                node.UnlockFromSaveSystem();
            }
        }

        foreach (var skill in SkillManager.allSkills)
        {
            if (data.skillUpgrades.TryGetValue(skill.GetSkillType(), out SkillUpgradeType upgradeType))
            {
                var upgradeNode = allTreeNodes.FirstOrDefault(
                    node => node.skillData.upgradeData.upgradeType == upgradeType
                );

                if (upgradeNode != null)
                {
                    skill.SetSkillUpgrade(upgradeNode.skillData);
                }
            }
        }
    }

    #endregion
}

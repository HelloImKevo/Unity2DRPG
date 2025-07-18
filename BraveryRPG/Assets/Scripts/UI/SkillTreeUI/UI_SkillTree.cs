using UnityEngine;

/// <summary>
/// Manages the skill tree system including skill point allocation, validation, and tree operations.
/// Controls the overall state and behavior of the skill progression system.
/// </summary>
public class UI_SkillTree : MonoBehaviour
{
    [SerializeField] private int skillPoints;
    [Tooltip("The top-most node in the tree hierarchy. Required for the UpdateAllConnections context menu action.")]
    [SerializeField] private UI_TreeConnectHandler[] parentNodes;

    public Player_SkillManager SkillManager { get; private set; }

    void Awake()
    {
        SkillManager = FindAnyObjectByType<Player_SkillManager>();
    }

    /// <summary>
    /// Initializes the skill tree on start.
    /// </summary>
    private void Start()
    {
        UpdateAllConnections();
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
    public void RemoveSkillPoints(int cost) => skillPoints -= cost;

    /// <summary>
    /// Adds skill points to the player's available pool.
    /// </summary>
    /// <param name="points">The number of skill points to add.</param>
    public void AddSkillPoints(int points) => skillPoints += points;

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
}

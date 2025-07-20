using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents a single skill node in the skill tree UI. Handles skill unlocking logic,
/// prerequisite validation, and user interaction through pointer events.
/// </summary>
public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;
    private UI_TreeConnectHandler connectHandler;

    [Header("Unlock Details")]
    public UI_TreeNode[] requiredNodes;
    public UI_TreeNode[] conflictNodes;

    [Space]
    [Tooltip("Whether this Skill has been unlocked by the player.")]
    public bool isUnlocked;

    /// <summary>
    /// Skill acquisition is blocked by an unfulfilled prerequisite.
    /// </summary>
    [Tooltip("Skill acquisition is blocked by an unfulfilled prerequisite.")]
    public bool isLocked;

    [Header("Skill Details")]
    [Tooltip("Skill data Scriptable Object containing details like the Name, Description and Cost.")]
    public Skill_DataSO skillData;

    // These fields are displayed in the Unity Editor for convenience.
    [SerializeField] private string skillName;
    [SerializeField] private int skillCost;

    [Tooltip("'Image' Game Object for the Skill Node Icon (not a sprite PNG).")]
    [SerializeField] private Image skillIcon;

    /// <summary>
    /// Tint applied to icon image when the skill is currently locked.
    /// </summary>
    [Tooltip("Tint applied to icon image when the skill is currently locked.")]
    [SerializeField] private string lockedColorHex = "#808080";

    // [Tooltip("Tint applied to icon image when the skill is currently locked.")]
    // [SerializeField] private Color skillLockedColor = Color.gray;

    private Color lastColor;

    /// <summary>Initializes component references and sets the initial icon color for the skill node.</summary>
    void Start()
    {
        UpdateIconColor(GetColorByHex(lockedColorHex));
        UnlockDefaultSkill();
    }

    public void UnlockDefaultSkill()
    {
        // Initialize 'Unlocked By Default' skills, because the SkillTree
        // gameObject is inactive by default, and won't invoked Start() lifecycle.
        InitNeededComponents();

        if (skillData.unlockedByDefault)
        {
            // Must be called after UI_SkillTree.Awake()
            Unlock();
        }
    }

    private void InitNeededComponents()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>(true);
        connectHandler = GetComponentInParent<UI_TreeConnectHandler>(true);
    }

    public void Refund()
    {
        if (!isUnlocked || skillData.unlockedByDefault)
        {
            // Do not refund skill points for skills that haven't been unlocked.
            return;
        }

        isUnlocked = false;
        isLocked = false;
        UpdateIconColor(GetColorByHex(lockedColorHex));

        skillTree.AddSkillPoints(skillData.cost);
        connectHandler.UnlockConnectionImage(false);
    }

    /// <summary>
    /// Unlocks this Skill for the player. Performs other post-unlock tasks,
    /// such as locking-out conflicting skills in the same Skill Tier.
    /// </summary>
    private void Unlock()
    {
        UpdateIconColor(Color.white);

        // When unlocking this skill, we need to lock-out all conflicting
        // skill nodes in the shared Skill Tier.
        LockOutConflictingNodes();

        if (isUnlocked)
        {
            Debug.Log($"{gameObject.name}.Unlock() -> Skill is already unlocked!");
            return;
        }

        isUnlocked = true;

        skillTree.RemoveSkillPoints(skillData.cost);
        connectHandler.UnlockConnectionImage(true);

        skillTree.SkillManager.GetSkillByType(skillData.skillType).SetSkillUpgrade(skillData);
    }

    /// <summary>
    /// Determines whether this skill node can be unlocked based on prerequisites,
    /// skill point availability, and conflict resolution.
    /// </summary>
    /// <returns>True if the skill can be unlocked, false otherwise.</returns>
    private bool CanBeUnlocked()
    {
        if (isLocked || isUnlocked)
        {
            return false;
        }

        if (!skillTree.EnoughSkillPoints(skillData.cost))
        {
            // Not enough skill points to unlock this Skill Node.
            return false;
        }

        foreach (var requiredNode in requiredNodes)
        {
            // If any of the Required Nodes have not been unlocked, then this
            // Skill Node cannot be unlocked.
            if (!requiredNode.isUnlocked) return false;
        }

        foreach (var conflictNode in conflictNodes)
        {
            // If there are any conflicting Nodes in the same Tier that have
            // already been unlocked, then this Skill is blocked (unless the
            // user does a Skill Respec).
            if (conflictNode.isUnlocked) return false;
        }

        return true;
    }

    /// <summary>
    /// Locks out all conflicting nodes.
    /// </summary>
    private void LockOutConflictingNodes()
    {
        foreach (var conflictNode in conflictNodes)
        {
            conflictNode.isLocked = true;
            // Recursively lock child nodes.
            conflictNode.LockChildNodes();
        }
    }

    public void LockChildNodes()
    {
        isLocked = true;

        foreach (var node in connectHandler.GetChildNodes())
        {
            node.LockChildNodes();
        }
    }

    /// <summary>
    /// Updates the visual color of the skill node icon.
    /// </summary>
    /// <param name="color">The color to apply to the skill icon.</param>
    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null) return;

        lastColor = skillIcon.color;
        skillIcon.color = color;
    }

    /// <summary>
    /// Handles pointer click events on the skill node. Attempts to unlock the skill
    /// if all prerequisites are met.
    /// </summary>
    /// <param name="eventData">Pointer event data from the UI system.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlocked())
        {
            Unlock();
        }
        else if (isLocked)
        {
            ui.skillTooltip.PlayLockedSkillBlinkEffect();
        }
        else
        {
            Debug.Log($"{gameObject.name} cannot be unlocked - Cost: {skillData.cost} points; Unlocked: {isUnlocked}");
        }
    }

    /// <summary>
    /// Handles pointer enter events to show the skill tooltip and highlight the node.
    /// </summary>
    /// <param name="eventData">Pointer event data from the UI system.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show tooltip and populate the text fields with skill data.
        ui.skillTooltip.ShowTooltip(true, rect, skillData, this);

        // Do not highlight locked skills (but still show the tooltip).
        if (CanBeUnlocked())
        {
            ToggleNodeHighlight(true);
        }
    }

    /// <summary>
    /// Handles pointer exit events to hide the skill tooltip and remove node highlighting.
    /// </summary>
    /// <param name="eventData">Pointer event data from the UI system.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        // Move the tooltip way off-screen.
        ui.skillTooltip.HideTooltip();

        if (CanBeUnlocked())
        {
            // Un-highlight the icon when the Mouse Pointer leaves.
            // We need to think about how this will work with Controllers;
            // there might be a library that snaps an invisible Cursor
            // to UI Buttons and Interactable components.
            ToggleNodeHighlight(false);
        }
    }

    private void ToggleNodeHighlight(bool shouldHighlight)
    {
        // Highlight the icon on Hover (when playing with KBM).
        // TODO: Implement UI control selection behavior for Controller inputs.
        Color highlightTint = Color.white * 0.9f;
        // Make the color fully opaque - no transparency.
        highlightTint.a = 1;
        Color colorToApply = shouldHighlight ? highlightTint : lastColor;

        UpdateIconColor(colorToApply);
    }

    /// <summary>
    /// Converts a hexadecimal color string to a Unity Color object.
    /// </summary>
    /// <param name="hexNumber">Hexadecimal color string (e.g., "#FF0000").</param>
    /// <returns>The corresponding Unity Color object.</returns>
    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);

        return color;
    }

    /// <summary>
    /// Restore skill node visual state when toggling the UI as shown / hidden.
    /// </summary>
    void OnDisable()
    {
        if (isLocked)
        {
            UpdateIconColor(GetColorByHex(lockedColorHex));
        }

        if (isUnlocked)
        {
            UpdateIconColor(Color.white);
        }
    }

    /// <summary>
    /// Called whenever the script's properties are set, including when an object is deserialized,
    /// which can occur at various times, such as when you open a scene in the Editor and after
    /// a domain reload.
    /// </summary>
    void OnValidate()
    {
        if (skillData == null) return;

        // Populate the Unity game object with properties derived from the Skill Data SO.
        skillName = skillData.displayName;
        skillCost = skillData.cost;
        skillIcon.sprite = skillData.icon;
        gameObject.name = "UI_TreeNode - " + skillData.displayName;
    }
}

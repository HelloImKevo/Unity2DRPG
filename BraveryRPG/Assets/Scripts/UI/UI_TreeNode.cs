using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Tooltip("Skill data Scriptable Object containing details like the Name, Description and Cost.")]
    [SerializeField] private Skill_DataSO skillData;

    [SerializeField] private string skillName;

    [Tooltip("'Image' Game Object for the Skill Node Icon (not a sprite PNG).")]
    [SerializeField] private Image skillIcon;

    /// <summary>
    /// Tint applied to icon image when the skill is currently locked.
    /// </summary>
    [Tooltip("Tint applied to icon image when the skill is currently locked.")]
    [SerializeField] private string lockedColorHex = "#808080";

    // [Tooltip("Tint applied to icon image when the skill is currently locked.")]
    // [SerializeField] private Color skillLockedColor = Color.gray;

    public bool isUnlocked;

    /// <summary>
    /// Skill acquisition is blocked by an unfulfilled prerequisite.
    /// </summary>
    public bool isLocked;

    private Color lastColor;

    void Awake()
    {
        UpdateIconColor(GetColorByHex(lockedColorHex));
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
        skillIcon.sprite = skillData.icon;
        gameObject.name = "UI_TreeNode - " + skillData.displayName;
    }


    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);

        // Find Player_SkillManager
        // Unlock skill on skill manager
        // Skill Manager unlock skill from skill data Type
    }

    private bool CanBeUnlocked()
    {
        if (isLocked || isUnlocked)
        {
            return false;
        }

        return true;
    }

    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null) return;

        lastColor = skillIcon.color;
        skillIcon.color = color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlocked())
        {
            Unlock();
        }
        else
        {
            Debug.Log("Cannot be unlocked");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isUnlocked)
        {
            // Highlight the icon on Hover.
            UpdateIconColor(Color.white * 0.9f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isUnlocked)
        {
            // Un-highlight the icon when the Mouse Pointer leaves.
            // We need to think about how this will work with Controllers;
            // there might be a library that snaps an invisible Cursor
            // to UI Buttons and Interactable components.
            UpdateIconColor(lastColor);
        }
    }

    private Color GetColorByHex(string hexNumber)
    {
        ColorUtility.TryParseHtmlString(hexNumber, out Color color);

        return color;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Tooltip("Image Game Object icon (not a sprite PNG).")]
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

    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);

        // Find Player_SkillManager
        // Unlock skill on skill manager
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

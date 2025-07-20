using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;
    private Image skillIcon;
    private RectTransform rect;
    private Button button;

    private Skill_DataSO skillData;

    public SkillType skillType;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private string inputKeyName;
    [SerializeField] private TextMeshProUGUI inputKeyText;
    [SerializeField] private GameObject conflictSlot;

    private Coroutine cooldownCoroutine;

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        button = GetComponent<Button>();
        skillIcon = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    private void OnValidate()
    {
        gameObject.name = "UI_SkillSlot - " + skillType.ToString();
    }

    public void SetupSkillSlot(Skill_DataSO selectedSkill)
    {
        this.skillData = selectedSkill;

        Color color = Color.black; color.a = 0.6f;
        cooldownImage.color = color;

        inputKeyText.text = inputKeyName;
        skillIcon.sprite = selectedSkill.icon;

        if (conflictSlot != null)
        {
            // Conflicting skill paths cannot be used - hide them from the skill bar UI.
            conflictSlot.SetActive(false);
        }
    }

    public void StartCooldownVisual(float cooldown)
    {
        cooldownImage.fillAmount = 1;
        cooldownCoroutine = StartCoroutine(CooldownCo(cooldown));
    }

    public void ResetCooldownVisual()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
        cooldownImage.fillAmount = 0;
    }

    private IEnumerator CooldownCo(float duration)
    {
        float timePassed = 0;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            // Update the transparent black radial overlay.
            cooldownImage.fillAmount = 1f - (timePassed / duration);
            yield return null;
        }

        cooldownImage.fillAmount = 0;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.HideTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillData == null) return;

        ui.skillTooltip.ShowTooltip(true, rect, skillData, null);
    }
}

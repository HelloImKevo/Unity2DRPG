using UnityEngine;

public class UI : MonoBehaviour
{
    public UI_SkillTooltip skillTooltip;

    void Awake()
    {
        skillTooltip = GetComponentInChildren<UI_SkillTooltip>();
    }
}

using UnityEngine;

/// <summary>
/// Main UI controller that manages references to various UI components and tooltips.
/// Acts as a central hub for UI element coordination.
/// </summary>
public class UI : MonoBehaviour
{
    [Tooltip("UI Elements that can be toggled between Active & Inactive.")]
    [SerializeField] private GameObject[] uiElements;

    public bool alternativeInput { get; private set; }
    private PlayerInputSet input;

    #region User Interface View Controllers
    public UI_SkillTree skillTreeUI { get; private set; }
    public UI_Inventory inventoryUI { get; private set; }
    public UI_Storage storageUI { get; private set; }
    public UI_Craft craftUI { get; private set; }
    public UI_Merchant merchantUI { get; private set; }
    public UI_InGame inGameUI { get; private set; }
    public UI_Options optionsUI { get; private set; }
    public UI_DeathScreen deathScreenUI { get; private set; }
    public UI_FadeScreen fadeScreenUI { get; private set; }

    public UI_SkillTooltip skillTooltip { get; private set; }
    public UI_ItemTooltip itemTooltip { get; private set; }
    public UI_StatTooltip statTooltip { get; private set; }
    #endregion

    private bool skillTreeEnabled;
    private bool inventoryEnabled;

    /// <summary>Initializes UI component references on awake.</summary>
    void Awake()
    {
        skillTreeUI = GetComponentInChildren<UI_SkillTree>(true);
        inventoryUI = GetComponentInChildren<UI_Inventory>(true);
        storageUI = GetComponentInChildren<UI_Storage>(true);
        craftUI = GetComponentInChildren<UI_Craft>(true);
        merchantUI = GetComponentInChildren<UI_Merchant>(true);
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        optionsUI = GetComponentInChildren<UI_Options>(true);
        deathScreenUI = GetComponentInChildren<UI_DeathScreen>(true);
        fadeScreenUI = GetComponentInChildren<UI_FadeScreen>(true);

        skillTooltip = GetComponentInChildren<UI_SkillTooltip>(true);
        itemTooltip = GetComponentInChildren<UI_ItemTooltip>(true);
        statTooltip = GetComponentInChildren<UI_StatTooltip>(true);

        // User Interface Panels
        if (skillTreeUI == null)
        {
            Debug.LogWarning("Skill Tree UI component is null, did you forget to assign it to the UI script?");
        }

        if (inventoryUI == null)
        {
            Debug.LogWarning("Inventory UI component is null, did you forget to assign it to the UI script?");
        }

        if (storageUI == null)
        {
            Debug.LogWarning("Storage UI component is null, did you forget to assign it to the UI script?");
        }

        if (craftUI == null)
        {
            Debug.LogWarning("Craft UI component is null, did you forget to assign it to the UI script?");
        }

        if (merchantUI == null)
        {
            Debug.LogWarning("Merchant UI component is null, did you forget to assign it to the UI script?");
        }

        if (inGameUI == null)
        {
            Debug.LogWarning("In-Game UI component is null, did you forget to assign it to the UI script?");
        }

        if (optionsUI == null)
        {
            Debug.LogWarning("Options UI component is null, did you forget to assign it to the UI script?");
        }

        if (deathScreenUI == null)
        {
            Debug.LogWarning("Death Screen UI component is null, did you forget to assign it to the UI script?");
        }

        // User Interface Tooltips
        if (skillTooltip == null)
        {
            Debug.LogWarning("Skill Tooltip component is null, did you forget to assign it to the UI script?");
        }

        if (itemTooltip == null)
        {
            Debug.LogWarning("Item Tooltip component is null, did you forget to assign it to the UI script?");
        }

        if (statTooltip == null)
        {
            Debug.LogWarning("Stat Tooltip component is null, did you forget to assign it to the UI script?");
        }

        // Fix bug where you have to press the UI input keybinding twice, in the
        // scenario where the UI component is already visible and active.
        skillTreeEnabled = skillTreeUI.gameObject.activeSelf;
        inventoryEnabled = inventoryUI.gameObject.activeSelf;
    }

    void Start()
    {
        skillTreeUI.UnlockDefaultSkills();
    }

    public void SetupControlsUI(PlayerInputSet inputSet)
    {
        input = inputSet;

        // Keyboard: L
        input.UI.SkillTreeUI.performed += ctx => ToggleSkillTreeUI();
        // Keyboard: C
        input.UI.InventoryUI.performed += ctx => ToggleInventoryUI();

        // Left CTRL Alternative Input (Buy Full Stack, etc.)
        input.UI.AlternativeInput.performed += ctx => alternativeInput = true;
        input.UI.AlternativeInput.canceled += ctx => alternativeInput = false;

        // Pause / Resume game.
        input.UI.OptionsUI.performed += ctx => OnEnterOptionsUIState();
    }

    private void OnEnterOptionsUIState()
    {
        foreach (var element in uiElements)
        {
            if (element.activeSelf)
            {
                // Resume game.
                GameManager.instance.UnpauseGame();
                SwitchToInGameUI();
                return;
            }
        }

        // Pause game when viewing Game Settings UI.
        GameManager.instance.PauseGame();
        OpenOptionsUI();
    }

    public void OpenDeathScreenUI()
    {
        SwitchTo(deathScreenUI.gameObject);
        // Pay attention to this if you use gamepad.
        // Buttons are not part of the Unity Input system, so you might
        // be able to simply use input.Disable()
        StopPlayerControls(true);
    }

    public void OpenOptionsUI()
    {
        HideAllTooltips();
        StopPlayerControls(true);
        SwitchTo(optionsUI.gameObject);
    }

    public void SwitchToInGameUI()
    {
        HideAllTooltips();
        StopPlayerControls(false);
        SwitchTo(inGameUI.gameObject);

        skillTreeEnabled = false;
        inventoryEnabled = false;
    }

    public void HideAllUI()
    {
        foreach (var element in uiElements)
        {
            element.SetActive(false);
        }
    }

    private void SwitchTo(GameObject objectToSwitchOn)
    {
        HideAllUI();

        objectToSwitchOn.SetActive(true);
    }

    private void StopPlayerControls(bool stopControls)
    {
        if (stopControls)
        {
            input.Player.Attack.Disable();
            input.Player.RangeAttack.Disable();
            input.Player.Jump.Disable();
            input.Player.Dash.Disable();
        }
        else
        {
            input.Player.Attack.Enable();
            input.Player.RangeAttack.Enable();
            input.Player.Jump.Enable();
            input.Player.Dash.Enable();
        }
    }

    private void StopPlayerControlsIfNeeded()
    {
        foreach (var element in uiElements)
        {
            if (element.activeSelf)
            {
                StopPlayerControls(true);
                return;
            }
        }

        StopPlayerControls(false);
    }

    public void ToggleSkillTreeUI()
    {
        skillTreeUI.transform.SetAsLastSibling();
        SetTooltipsAsLastSibling();
        // Fade Screen should hide / cover all other UI elements.
        fadeScreenUI.transform.SetAsLastSibling();

        skillTreeEnabled = !skillTreeEnabled;
        // Activate / Deactivate game objects.
        skillTreeUI.gameObject.SetActive(skillTreeEnabled);
        // Hide the tooltip (move it into outer space).
        skillTooltip.HideTooltip();

        StopPlayerControlsIfNeeded();
    }

    public void ToggleInventoryUI()
    {
        // Move the Inventory UI to the top of the view draw stack,
        // except for Tooltips, which should always be rendered on top.
        inventoryUI.transform.SetAsLastSibling();
        SetTooltipsAsLastSibling();
        // Fade Screen should hide / cover all other UI elements.
        fadeScreenUI.transform.SetAsLastSibling();

        inventoryEnabled = !inventoryEnabled;
        // Activate / Deactivate game objects.
        inventoryUI.gameObject.SetActive(inventoryEnabled);
        // Hide the tooltips (move them into outer space).
        HideAllTooltips();

        StopPlayerControlsIfNeeded();
    }

    public void OpenStorageUI(bool openStorageUI)
    {
        storageUI.gameObject.SetActive(openStorageUI);
        StopPlayerControls(openStorageUI);

        if (!openStorageUI)
        {
            craftUI.gameObject.SetActive(false);
            HideAllTooltips();
        }
    }

    public void OpenMerchantUI(bool openMerchantUI)
    {
        merchantUI.gameObject.SetActive(openMerchantUI);
        StopPlayerControls(openMerchantUI);

        if (!openMerchantUI)
        {
            HideAllTooltips();
        }
    }

    public void HideAllTooltips()
    {
        itemTooltip.HideTooltip();
        skillTooltip.HideTooltip();
        statTooltip.HideTooltip();
    }

    private void SetTooltipsAsLastSibling()
    {
        itemTooltip.transform.SetAsLastSibling();
        skillTooltip.transform.SetAsLastSibling();
        statTooltip.transform.SetAsLastSibling();
    }
}

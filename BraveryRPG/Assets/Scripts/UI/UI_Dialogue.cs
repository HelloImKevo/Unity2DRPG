using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dialogue : MonoBehaviour
{
    private UI ui;
    // private DialogueNpcData npcData;
    private Player_QuestManager questManager;

    [SerializeField] private Image speakerPortrait;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI[] dialogueChoicesText;

    [Space]
    [Tooltip("20 = fast, 5 = slow")]
    [SerializeField, Min(1f)] private float charactersPerSecond = 20f;
    private string fullTextToShow;
    private Coroutine typeTextCo;

    private DialogueLineSO currentLine;
    // TODO: Improve design by using a dedicated DialogueChoiceSO
    private DialogueLineSO[] currentChoices;
    private DialogueLineSO selectedChoice;
    private int selectedChoiceIndex;

    private bool waitingToConfirm;
    private bool canInteract;

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        questManager = Player.GetInstance().questManager;

        canInteract = true;
    }

    // public void SetupNpcData(DialogueNpcData npcData) => this.npcData = npcData;

    public void PlayDialogueLine(DialogueLineSO line)
    {
        if (typeTextCo != null && selectedChoice != null)
        {
            // TODO: Come up with a more robust canInteract check - I think there's
            // an issue with the EnableInteractionCo implementation.
            CompleteTyping();
            return;
        }

        currentLine = line;
        currentChoices = line.choiceLines;
        canInteract = false;
        selectedChoice = null;
        selectedChoiceIndex = 0;

        HideAllChoices();

        speakerPortrait.sprite = line.speakerData.speakerPortrait;
        speakerName.text = line.speakerData.speakerName;

        fullTextToShow = line.actionType == DialogueActionType.None || line.actionType == DialogueActionType.PlayerMakeChoice
            ? line.GetRandomLine()
            : line.npcResponseText;

        Debug.Log($"PlayDialogueLine() -> fullTextToShow = {fullTextToShow}");
        typeTextCo = StartCoroutine(TypeTextCo(fullTextToShow));
        StartCoroutine(EnableInteractionCo());
    }

    private void HandleNextAction()
    {
        switch (currentLine.actionType)
        {
            case DialogueActionType.OpenShop:
                ui.SwitchToInGameUI();
                ui.OpenMerchantUI(true);
                break;

            case DialogueActionType.PlayerMakeChoice:
                if (selectedChoice == null)
                {
                    ShowChoices();
                }
                else
                {
                    // Activate the selected dialogue choice when the player
                    // clicks the 'Interact' (F) key.
                    DialogueLineSO selectedChoice = currentChoices[selectedChoiceIndex];
                    PlayDialogueLine(selectedChoice);
                }
                break;

            case DialogueActionType.OpenQuest:
                ui.SwitchToInGameUI();
                // ui.OpenQuestUI(npcData.quests);
                break;

            case DialogueActionType.GetQuestReward:
                ui.SwitchToInGameUI();
                // questManager.TryGetRewardFrom(npcData.npcRewardType);
                break;

            case DialogueActionType.OpenCraft:
                ui.SwitchToInGameUI();
                // ui.OpenCraftUI(true);
                break;

            case DialogueActionType.CloseDialogue:
                ui.SwitchToInGameUI();
                break;
        }
    }

    public void DialogueInteraction()
    {
        if (!canInteract) return;

        if (typeTextCo != null)
        {
            // If player re-interacts, auto-complete the dialogue output.
            CompleteTyping();

            if (currentLine.actionType != DialogueActionType.PlayerMakeChoice)
            {
                waitingToConfirm = true;
            }
            else
            {
                HandleNextAction();
            }

            return;
        }

        if (waitingToConfirm || selectedChoice != null)
        {
            waitingToConfirm = false;
            HandleNextAction();
        }
    }

    /// <summary>Auto-complete and finish the typewriter text output.</summary>
    private void CompleteTyping()
    {
        if (typeTextCo != null)
        {
            StopCoroutine(typeTextCo);
            Debug.Log($"CompleteTyping() -> fullTextToShow = {fullTextToShow}");
            dialogueText.text = fullTextToShow;
            typeTextCo = null;
        }
    }

    private void ShowChoices()
    {
        for (int i = 0; i < dialogueChoicesText.Length; i++)
        {
            if (i < currentChoices.Length)
            {
                DialogueLineSO choice = currentChoices[i];
                string choiceText = choice.playerChoiceAnswer;

                dialogueChoicesText[i].gameObject.SetActive(true);
                dialogueChoicesText[i].text = selectedChoiceIndex == i
                    ? $"<color=yellow> {i + 1}) {choiceText}"
                    : $"{i + 1}) {choiceText}";

                // if (choice.actionType == DialogueActionType.GetQuestReward
                //     && !questManager.HasCompletedQuest())
                // {
                //     dialogueChoicesText[i].gameObject.SetActive(false);
                // }
            }
            else
            {
                dialogueChoicesText[i].gameObject.SetActive(false);
            }
        }

        selectedChoice = currentChoices[selectedChoiceIndex];

        Debug.Log($"ShowChoices() -> selectedChoice is now = {selectedChoice}");
    }

    private void HideAllChoices()
    {
        foreach (var obj in dialogueChoicesText)
        {
            obj.gameObject.SetActive(false);
        }
    }

    public void NavigateChoice(int direction)
    {
        // Uses the Positive/Negative Binding for W and S to navigate
        // up and down between the dialogue choices.
        if (currentChoices == null || currentChoices.Length <= 1) return;

        selectedChoiceIndex += direction;
        selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, currentChoices.Length - 1);
        ShowChoices();
    }

    private IEnumerator TypeTextCo(string text)
    {
        dialogueText.text = "";

        float delay = 1f / charactersPerSecond;

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(delay);
        }

        if (currentLine.actionType != DialogueActionType.PlayerMakeChoice)
        {
            waitingToConfirm = true;
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
            selectedChoice = null;
            HandleNextAction();
        }

        typeTextCo = null;
    }

    private IEnumerator EnableInteractionCo()
    {
        yield return null;
        canInteract = true;
    }
}

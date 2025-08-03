using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Dialogue Data/New Dialogue Line Data", fileName = "Dialogue Line - ")]
public class DialogueLineSO : ScriptableObject
{
    [Header("Dialogue Info")]
    public string dialogueGroupName;
    public string dialogueDescription;
    public DialogueSpeakerSO speakerData;

    [Space]
    [Header("Text Options")]
    [Tooltip("These are text spoken by the NPC.")]
    [TextArea] public string[] textLines;

    [Header("Player Choices")]
    [Tooltip("This is used for player answers & choices.")]
    [TextArea] public string playerChoiceAnswer;
    [Space]
    public DialogueLineSO[] choiceLines;

    [Header("Dialogue Action")]
    [Tooltip("This will be said by the NPC before the action is triggered.")]
    [TextArea] public string actionLine;
    public DialogueActionType actionType;

    public string GetFirstLine() => textLines[0];

    public string GetRandomLine()
    {
        return textLines[Random.Range(0, textLines.Length)];
    }
}

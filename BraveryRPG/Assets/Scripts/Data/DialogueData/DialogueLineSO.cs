using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Dialogue Data/New Dialogue Line Data", fileName = "Dialogue Line - ")]
public class DialogueLineSO : ScriptableObject
{
    [Header("Dialogue Info")]
    public string dialogueGroupName;
    public DialogueSpeakerSO speakerData;

    [Space]
    [Header("Text Options")]
    [TextArea] public string[] textLines;

    [Header("Choices Info")]
    [TextArea] public string playerChoiceAnswer;
    [Space]
    public DialogueLineSO[] choiceLines;

    [Header("Dialogue Action")]
    [TextArea] public string actionLine;
    public DialogueActionType actionType;

    public string GetFirstLine() => textLines[0];

    public string GetRandomLine()
    {
        return textLines[Random.Range(0, textLines.Length)];
    }
}

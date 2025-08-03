using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Dialogue Data/New Dialogue Choice Data", fileName = "Dialogue Choice - ")]
public class DialogueChoiceSO : ScriptableObject
{
    // TODO: Supporting this construct would require a bit of refactoring to the UI_Dialogue.
    [TextArea] public string choiceText;
    public DialogueActionType actionType;
    [TextArea] public string npcResponseText;
}

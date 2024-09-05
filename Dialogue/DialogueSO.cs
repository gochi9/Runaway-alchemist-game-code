using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "DialogueSystem/Dialogue")]
public class DialogueSO : ScriptableObject
{
    public string dialogueId;
    public List<string> lines = new ();
    public List<DialogueButtonType> dialogueButtonTypes = new ();
    public DialogueSO nextDialogue;
    public DialogueAction dialogueAction;
}
using UnityEngine;

public class DialogueCollider : MonoBehaviour
{
    public DialogueManager dialogueTextHandler;
    public bool canClick = true;

    void OnMouseDown(){
        if(!canClick)
            return;
            
        dialogueTextHandler.onUserClick();
    }
}

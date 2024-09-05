using UnityEngine;

public class DialogueButtonHandler : MonoBehaviour
{
    public DialogueCollider dialogueCollider;
    private  DialogueManager dialogueManager;
    private DayCounter dayCounter;
    public PotScript potView;

    void Awake(){
        dialogueManager = GetComponent<DialogueManager>();
        dayCounter = GetComponent<DayCounter>();
    }

    public void repeatThis(){
        dialogueManager.endDialogue();
        dialogueCollider.canClick = true;  
        dialogueManager.clearExistingButtons();
    }

    public void discardPotion(){
        potView.resetPotion();
        dialogueManager.startDialogue("POTION_DISCARD");
    }

    public void tastePotion(){
        potView.tastePotion();
    }

    public void finishPotion(){
        potView.finishPotion();
    }

    public void failPotion(){
        potView.resetPotion();
    }
}

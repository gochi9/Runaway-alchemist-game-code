using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Transform buttonContainer;
    public Button buttonPrefab;
    public List<DialogueSO> allDialogues = new ();
    private Dictionary<string, DialogueSO> dialogueDictionary = new ();
    private DialogueSO currentDialogue;
    private int currentLineIndex;
    public float typingSpeed = 0.05f;
    public Vector3 buttonStartPosition;
    public float buttonVerticalSpacing;

    // DialogueTextHandler variables
    public bool isCurrentlyInDialogue = false;
    public GameObject dialogueHolder;
    public GameObject dialogueFrame1, dialogueFrame2;
    public DialogueCollider clickCollider;
    public TextMeshPro textMeshPro;
    private bool first = true;
    private bool isTypingText = false;

    private DayCounter dayCounter;
    private DialogueButtonHandler dialogueButtonHandler;
    public string potionPlaceholder = "error", rarityPlaceholder = "error"; 

    void Awake() {
        dayCounter = GetComponent<DayCounter>();
        dialogueButtonHandler = GetComponent<DialogueButtonHandler>();
        foreach (DialogueSO dialogue in allDialogues) {
            dialogueDictionary.Add(dialogue.dialogueId, dialogue);
            dialogueDictionary[dialogue.dialogueId] = dialogue;    
        }
        
        clickCollider.canClick = false;
    }

    public void startDialogue(string dialogueId) {
        try{
            currentDialogue = dialogueDictionary[dialogueId];
            initializeDialogue();
            startTyping();
        }
        catch(Exception){}
    }

    public void startDialogue(DialogueSO dialogueSO) {
        try{
            currentDialogue = dialogueSO;
            initializeDialogue();
            startTyping();
        }
        catch(Exception){}
    }

    private string currentDialogueId;
    public void startDialogue() {
        try{
            currentDialogue = dialogueDictionary[currentDialogueId];
            initializeDialogue();
            startTyping();
        }
        catch(Exception){}

        currentDialogueId = null;
    }

    private void initializeDialogue() {
        currentLineIndex = 0;
        isCurrentlyInDialogue = true;
        clickCollider.canClick = true;
        dialogueHolder.SetActive(true);
        dialogueFrame1.SetActive(true);
        dialogueFrame2.SetActive(false);
        textMeshPro.text = null;
    }

    public void onUserClick() {
        if (!isCurrentlyInDialogue) 
            return;

        if (isTypingText) {
            StopAllCoroutines();
            textMeshPro.text = new string(currentDialogue.lines[currentLineIndex]).Replace("[potionNAMEPlaceholder]", potionPlaceholder).Replace("[potionRARITYPlaceholder]", rarityPlaceholder);;
            isTypingText = false;
            return;
        } 
        
        if (++currentLineIndex < currentDialogue.lines.Count) 
                startTyping();
            else 
                handleLineEnd();
    }

    private void startTyping() {
        string text = new string(currentDialogue.lines[currentLineIndex]).Replace("[potionNAMEPlaceholder]", potionPlaceholder).Replace("[potionRARITYPlaceholder]", rarityPlaceholder);
        adjustFontSize(text);
        StartCoroutine(typeText(text));
    }

    private IEnumerator typeText(string text) {
        isTypingText = true;

        foreach (char letter in text.ToCharArray()) {
            textMeshPro.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingText = false;
    }

    private void handleLineEnd() {
        if (currentDialogue.dialogueButtonTypes.Count > 0) {
            createButtons(currentDialogue.dialogueButtonTypes);
            clickCollider.canClick = false;
        } 
        else if(currentDialogue.dialogueAction != DialogueAction.NONE)
            handleAction();
        else if (currentDialogue.nextDialogue != null)
            StartCoroutine(startNextDialogue());
        else 
            endDialogue();
    }

    private IEnumerator startNextDialogue() {
        yield return new WaitForSeconds(0.1f);
        startDialogue(currentDialogue.nextDialogue.dialogueId);
    }

    private void createButtons(List<DialogueButtonType> buttonTypes) {
        clearExistingButtons();
        Vector3 currentPosition = buttonStartPosition;

        foreach (var buttonType in buttonTypes) {
            var button = Instantiate(buttonPrefab, buttonContainer);
            button.transform.localPosition = currentPosition;
            currentPosition.y += buttonVerticalSpacing;
            addButtonAction(button, buttonType);
        }
    }

    public void clearExistingButtons() {
        foreach (Transform child in buttonContainer) {
            Destroy(child.gameObject);
        }
    }

    private void addButtonAction(Button button, DialogueButtonType buttonType) {
        button.onClick.AddListener(() => dialogueButtonHandler.repeatThis());
        switch (buttonType) {
            case DialogueButtonType.DISCARD_POTION:
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Discard";
                button.onClick.AddListener(() => dialogueButtonHandler.discardPotion());
                break;
            case DialogueButtonType.TASTE_POTION_START:
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Taste";
                button.onClick.AddListener(() => dialogueButtonHandler.tastePotion());
                break;
            case DialogueButtonType.FINISH_POTION:
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Finish";
                button.onClick.AddListener(() => dialogueButtonHandler.finishPotion());
                break;
        }
    }

    private void handleAction(){
        endDialogue();
        switch(currentDialogue.dialogueAction){
            case DialogueAction.START_INTRO_PART_2:
                currentDialogueId = "INTRO_PART_2";
                dayCounter.forceFadeIn();
                isCurrentlyInDialogue = true;
                Invoke("startDialogue", 1);
                break;
            case DialogueAction.POTION_FAILED:
                dialogueButtonHandler.failPotion();
                break;
        }
    }

    public void endDialogue() {
        dialogueHolder.SetActive(false);
        dialogueFrame1.SetActive(false);
        dialogueFrame2.SetActive(false);
        clickCollider.canClick = false;
        isCurrentlyInDialogue = false;
        textMeshPro.text = "";
        clearExistingButtons();
    }

    public void switchScene() {
        dialogueFrame1.SetActive(first);
        dialogueFrame2.SetActive(!first);
        first = !first;
    }

    private void adjustFontSize(string text) {
        textMeshPro.enableAutoSizing = true;
        textMeshPro.text = text;
        textMeshPro.ForceMeshUpdate();
        float preferredFontSize = textMeshPro.fontSize;
        textMeshPro.enableAutoSizing = false;
        textMeshPro.fontSize = preferredFontSize;
        textMeshPro.text = "";
    }

    public DialogueSO GetDialogueSO(string id){
        try{
            return dialogueDictionary[id];
        }
        catch(Exception){return null;}
    }
}

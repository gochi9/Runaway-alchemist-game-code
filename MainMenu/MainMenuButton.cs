using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    public MainMenuButtonType mainMenuButtonType;
    public Sprite normal, onHold;
    private SpriteRenderer spriteRenderer;
    private MainMenuScript mainMenuScript;

    public void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainMenuScript = transform.parent.GetComponent<MainMenuScript>();
        spriteRenderer.sprite = normal;
    }

    public void OnMouseOver(){
       if(mainMenuButtonType == MainMenuButtonType.CONTINUE && !PlayerPrefs.HasKey("currentDay"))
            return;

        spriteRenderer.sprite = onHold;
    }

    public void OnMouseExit(){
        spriteRenderer.sprite = normal;
    }

    public void OnMouseDown(){
        switch(mainMenuButtonType){
            case MainMenuButtonType.START:
                mainMenuScript.startNewGame();
                break;
            case MainMenuButtonType.CONTINUE:
                mainMenuScript.continueGame();
                break;
            case MainMenuButtonType.CREDITS:
                mainMenuScript.credits();
                OnMouseExit();
                break;
        }
    }
}

public enum MainMenuButtonType
{
    START,
    CONTINUE,
    CREDITS,
    LEAVE_CREDITS
}

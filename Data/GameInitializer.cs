using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameInitializer : MonoBehaviour{

    private DayCounter dayCounter;
    private DialogueManager dialogueManager;
    private StoreData storeData;

    void Start(){
        dayCounter = GetComponent<DayCounter>();
        dayCounter.CHANGING_DAY = true;
        storeData = GetComponent<StoreData>();
        noCandle();
    }

    void OnEnable(){
        dialogueManager = GetComponent<DialogueManager>();
        Invoke("test", 2f);
    }

    void test(){
        bool isNewGame = MainMenuScript.isNewGame;
        Debug.Log(isNewGame);

        if(!isNewGame && PlayerPrefs.HasKey("currentDay"))
            storeData.loadGame();
        else
            dialogueManager.startDialogue("INTRO_PART_1");
    }

    private void noCandle(){
        Light2D light = dayCounter.main_room_candle.GetComponent<Light2D>();

        light.pointLightInnerRadius = 0f;
        light.pointLightOuterRadius = 0f;
        light.enabled = false;
    }

}
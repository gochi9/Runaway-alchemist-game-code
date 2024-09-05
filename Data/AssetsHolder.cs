using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class AssetsHolder : MonoBehaviour
{
    public static Texture2D cursor1, cursor2;
    public Texture2D cursor_1, cursor_2;
    public List<FrameHolder> assets = new();
    public FrameHolder currentScene;
    public float minDelay, maxDelay;
    public long sceneChangeDelay;
    public List<ResourcesHolder> resourcesHolders = new ();
    private bool running = true;
    private System.Random random;
    private int currentDay;
    private long sceneChangeCurrentCooldown;
    private DayCounter dayCounter;
    private DialogueManager dialogueManager;

    void Start(){
        if(assets.Count < 1 && currentScene == null){
            Debug.Log("ADD THE BACKGROUND. FUCKKKKKKKKK");
            return;
        }

        cursor1 = cursor_1;
        cursor2 = cursor_2;

        random = new System.Random();

        if(minDelay <= 0.0f)
            minDelay = 1.0f;

        foreach(FrameHolder frame in assets){
            frame.gameObject.SetActive(true);
            frame.gameObject.SetActive(false);
        }

        if(currentScene == null)
            currentScene = assets[0];

        dayCounter = GetComponent<DayCounter>();
        dialogueManager = GetComponent<DialogueManager>();
        currentScene.gameObject.SetActive(true);
        StartCoroutine(CoroutineTask());
    }

    void Update(){
        Debug.Log(dayCounter.CHANGING_DAY + " " + dialogueManager.isCurrentlyInDialogue + " " + (sceneChangeCurrentCooldown >= DateTimeOffset.Now.ToUnixTimeMilliseconds()));
        if(dayCounter.CHANGING_DAY || dialogueManager.isCurrentlyInDialogue ||  sceneChangeCurrentCooldown >= DateTimeOffset.Now.ToUnixTimeMilliseconds())
            return;

        if(Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(0);

        if(Input.GetKeyDown(KeyCode.K))
            dayCounter.advanceDay();

        handleSceneChange(KeyCode.W, currentScene.forwardScene);
        handleSceneChange(KeyCode.S, currentScene.backScene);
        handleSceneChange(KeyCode.A, currentScene.leftScene);
        handleSceneChange(KeyCode.D, currentScene.rightScene);
    }

    void OnDisable(){
        running = false;
        StopCoroutine(CoroutineTask());
    }


    IEnumerator CoroutineTask(){
        while(running){
            if(currentScene != null)
                currentScene.switchFrames();

            if(dialogueManager.isCurrentlyInDialogue)
                dialogueManager.switchScene();

            yield return new WaitForSeconds(getRandomInterval());
        }
    }

    public void handleSceneChange(KeyCode key, GameObject newScene) {
        if (newScene == null || !Input.GetKeyDown(key))
            return;

        currentScene.gameObject.SetActive(false);
        currentScene = newScene.GetComponent<FrameHolder>();
        currentScene.gameObject.SetActive(true);
        sceneChangeCurrentCooldown = DateTimeOffset.Now.ToUnixTimeMilliseconds() + sceneChangeDelay;
        currentScene.tutorial();
    }

    public void test(){
        Debug.Log("Test");
    }

    private float getRandomInterval(){
        if(minDelay >= maxDelay)
            return minDelay;

        return (float)(random.NextDouble() * (maxDelay - minDelay) + minDelay);;
    }

}

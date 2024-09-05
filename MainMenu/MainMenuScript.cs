using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public SpriteRenderer frame_1, frame_2;

    public static bool isNewGame { get; private set;}

    public bool isCredits = false;
    public GameObject cred;

    void Awake(){
        working = true;
        StartCoroutine(swtichFrames());
    }

    void OnDisable(){
        working = false;
        StopCoroutine(swtichFrames());
    }

    public void startNewGame(){
        isNewGame = true;
        SceneManager.LoadScene(1);
    }

    public void continueGame(){
        if(!PlayerPrefs.HasKey("currentDay"))
            return;

        isNewGame = false;
        SceneManager.LoadScene(1);
    }

    public void credits(){
        foreach(Transform child in transform)
            child.gameObject.SetActive(false);

        isCredits = true;
        cred.SetActive(true);
    }

    void Update(){
        if(!isCredits || !(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.S)))
            return;

        foreach(Transform child in transform)
            child.gameObject.SetActive(true);

        isCredits = false;
        cred.SetActive(false);
    }

    private bool first, working;
    private IEnumerator swtichFrames(){
        while(working){
            frame_1.enabled = first;
            frame_2.enabled = !first;

            first = !first;
            yield return new WaitForSeconds(1f);
        }
    }
}

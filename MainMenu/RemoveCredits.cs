using UnityEngine;

public class RemoveCredits : MonoBehaviour
{   
    public MainMenuScript mainMenuScript;

    void OnMouseDown(){
        foreach(Transform child in mainMenuScript.transform)
            child.gameObject.SetActive(true);

        mainMenuScript.isCredits = false;
        mainMenuScript.cred.SetActive(false);
    }
}

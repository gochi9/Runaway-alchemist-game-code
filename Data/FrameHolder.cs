using System;
using System.Collections.Generic;
using UnityEngine;

public class FrameHolder : MonoBehaviour
{
    public List<GameObject> frame_1 = new(), frame_2 = new();
    public GameObject leftScene, rightScene, forwardScene, backScene;
    public int lastKnownDay { get; private set; } = 0;
    public bool first = true;
    public string dialogueID;
    public DialogueManager dialogueManager;
    public bool firstEnable = true;

    public void switchFrames(){        
        foreach(GameObject frame in frame_1)
            frame.SetActive(first);

        foreach(GameObject frame in frame_2)
            frame.SetActive(!first);

        first = !first;
    }

    public void tutorial(){
        Debug.Log(firstEnable);
        if(firstEnable){
            firstEnable = false;
            if(!(String.IsNullOrWhiteSpace(dialogueID) || dialogueID.Equals("MAINHALL")))
                dialogueManager.startDialogue(dialogueID);
        }
    }
}

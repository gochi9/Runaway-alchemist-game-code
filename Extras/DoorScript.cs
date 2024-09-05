using System;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public AssetsHolder parentHolder;
    public GameObject scene;
    public DayCounter dayCounter;

    void Start(){
        dayCounter = FindObjectOfType<DayCounter>();

        if(parentHolder != null)
            return;

        if(!findParent(transform))
            Debug.Log("What in the fuck is this gameobject attached to??? /////////" + this);
    }

    void OnMouseDown() {
        if(!dayCounter.CHANGING_DAY && scene != null && parentHolder != null)
            parentHolder.handleSceneChange(KeyCode.Mouse0, scene);
    }

    private bool findParent(Transform pos){
        Transform par = pos.parent;
        
        if(par == null)
            return false;

        this.parentHolder = par.GetComponent<AssetsHolder>();

        return (parentHolder != null) ? true : findParent(par);
    }

    public void OnMouseOver(){
        Cursor.SetCursor(AssetsHolder.cursor2, Vector2.zero, CursorMode.Auto);
    }

    public void OnMouseExit(){
        Cursor.SetCursor(AssetsHolder.cursor1, Vector2.zero, CursorMode.Auto);
    }

    public void OnDisable(){
        OnMouseExit();
    }
}

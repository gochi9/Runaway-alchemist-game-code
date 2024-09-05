using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarCollider : MonoBehaviour
{
    public MortarView mortarView;
    private Camera mainCamera;

    void OnEnable(){
        mainCamera = Camera.main;
    }

    public void OnMouseDown(){
        Vector3 mouseScreenPosition = Input.mousePosition;
        mortarView.clickMouse(mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, -3f)));
    }
}

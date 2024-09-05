using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollider : MonoBehaviour
{
    public GrindObject grindObject;
    public InventoryManager inventoryManager;
    public void OnMouseDown(){
        if(inventoryManager.currentSelectedItem != null && inventoryManager.currentSelectedItem.Equals("PISTIL"))
            return;

        grindObject.onClick();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FermentationCollider : MonoBehaviour
{
    private FermentationView fermentationView;
    public InventoryManager inventoryManager;

    void Awake(){
        fermentationView = transform.parent.GetComponent<FermentationView>();
    }

    void OnMouseDown(){
        string currentItem = inventoryManager.currentSelectedItem;
        
        inventoryManager.currentSelectedItem = null;
    
        if(fermentationView.state == 2){
            fermentationView.retrieveItems();
            return;
        }

        float currentAmount = inventoryManager.selectedDropAmount;

        if(!(currentAmount > 0.0f && currentItem != null && currentItem.Length > 0))
            return;

        fermentationView.addItem(currentItem, currentAmount);
    }
}

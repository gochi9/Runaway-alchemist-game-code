using UnityEngine;

public class DistillerCollider : MonoBehaviour
{
    private DistillerView distillerView;
    public InventoryManager inventoryManager;

    void Awake(){
        distillerView = transform.parent.GetComponent<DistillerView>();
    }

    void OnMouseDown(){
        string currentItem = inventoryManager.currentSelectedItem;
        
        inventoryManager.currentSelectedItem = null;
        
        if(distillerView.started)
            return;

        float currentAmount = inventoryManager.selectedDropAmount;

        if(!(currentAmount > 0.0f && currentItem != null && currentItem.Length > 0))
            return;

        distillerView.currentSelectedItem = currentItem;
        distillerView.amountSelected = currentAmount;
        distillerView.startMinigame();
    }
}

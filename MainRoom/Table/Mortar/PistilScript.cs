using UnityEngine;

public class PistilScript : MonoBehaviour
{
    private InventoryManager inventoryManager;

    void Awake(){
        inventoryManager = transform.parent.GetComponent<InventoryManager>();
    }

    void OnMouseDown(){
        if(inventoryManager.currentSelectedItem != null)
            inventoryManager.currentSelectedItem = null;
        else
            inventoryManager.currentSelectedItem = "PISTIL";
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

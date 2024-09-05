using UnityEngine;

public class TriggerSortInventory : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Coords coords;

    public void OnMouseDown(){
        inventoryManager.sortInventory(coords);
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

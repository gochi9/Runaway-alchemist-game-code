
using UnityEngine;

public class ResourcesData : MonoBehaviour
{
    private DataHolder dataHolder;
    private BoxCollider2D boxCollider;
    public string ID;
    public float amount;
    public float minAmount, maxAmount, minAmountBonus, maxAmountBonus;

    void Awake(){
        boxCollider = GetComponent<BoxCollider2D>();
        minAmount = Mathf.Max(minAmount, 0);
        maxAmount = Mathf.Max(maxAmount, 1);
        minAmountBonus = Mathf.Max(minAmountBonus, 0);
        maxAmountBonus = Mathf.Max(maxAmountBonus, 1);
        refresh();

        Transform ultimateParent = transform;
        while (ultimateParent.parent != null)
            ultimateParent = ultimateParent.parent;

        if(ultimateParent != null)
            dataHolder = ultimateParent.GetComponent<DataHolder>();
    }

    public void refresh(){
        gameObject.SetActive(amount > 0.0f);
    }

    public void OnMouseDown() {
        resourceClick(boxCollider.bounds.center);
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

    public void resourceClick(Vector2 pos){
        if(dataHolder != null){
            dataHolder.modifyInventory(ID, amount, DataModifyType.ADD);
            dataHolder.showFloatingText("+" + PotScript.formatDouble(amount) + " g", pos);
        }

        amount = 0;
        refresh();
    }
}

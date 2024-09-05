using System;
using System.Collections.Generic;
using UnityEngine;

public class MortarView : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public List<GrindObject> grindObjects = new ();
    public DataHolder dataHolder;
    public ParticleSystem dustEffectPrefab;

    public void clickMouse(Vector3 pos){
        string id = inventoryManager.currentSelectedItem;

        inventoryManager.currentSelectedItem = null;

        if(id == null || id.Length < 1 || id.Equals("PISTIL"))
            return;

        float amount = inventoryManager.selectedDropAmount;

        if(amount <= 0.0f)
            return;

        if(amount > dataHolder.getItemAmount(id))
            amount = dataHolder.getItemAmount(id);

        pos.z = -3f;
        GameObject obj = new GameObject(DateTimeOffset.Now.ToUnixTimeMilliseconds()+"");
        obj.AddComponent<SpriteRenderer>().sprite = dataHolder.GetSprite(id);
        obj.AddComponent<BoxCollider2D>();
        GrindObject grindObject = obj.AddComponent<GrindObject>();
        grindObject.id = id;
        grindObject.amount = amount;
        grindObject.originalAmount = amount;
        grindObject.inventoryManager = inventoryManager;
        grindObject.mortarView = this;
        obj.transform.parent = this.transform;
        obj.transform.position = pos;
        grindObjects.Add(grindObject);
        dataHolder.modifyInventory(id, amount, DataModifyType.REMOVE);
        inventoryManager.refreshInventory();
    }

    public void OnDisable(){
        foreach(GrindObject grindObject in grindObjects){
            if(grindObject.amount <= 0.0f || grindObject.id == null || grindObject.id.Length < 1){
                Destroy(grindObject.gameObject);
                continue;
            }

            dataHolder.modifyInventory(grindObject.id, grindObject.amount, DataModifyType.ADD);
            Destroy(grindObject.gameObject);
        }

        grindObjects.Clear();
    }

    public void destroyGrindObject(GrindObject grindObject){
        if(!(grindObject.amount <= 0.0f || grindObject.id == null || grindObject.id.Length < 1))
            dataHolder.modifyInventory(grindObject.id, grindObject.amount, DataModifyType.ADD);

        grindObjects.Remove(grindObject);
        Destroy(grindObject.gameObject);
        inventoryManager.refreshInventory();
    }
}

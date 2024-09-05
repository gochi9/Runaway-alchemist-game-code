using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

public class BigNote : MonoBehaviour
{
    public TextMeshPro text;
    public SpriteRenderer potionPrefab;
    public note note;
    public DataHolder dataHolder;
    public InventoryManager inventoryManager;
    private CrateView crateView;

    public void OnEnable(){
        inventoryManager.currentSelectedItem = null;
        crateView = transform.parent.GetComponent<CrateView>();
        text.text = note.description.Replace("[min]", note.mild_rarity+"").Replace("[max]", note.miracle_rarity+"");
        refreshBigNote();
    }

    public void refreshBigNote(){ 
        potionPrefab.enabled = !(note.id == null || note.id.Length < 1);

        if(potionPrefab.enabled)
            potionPrefab.sprite = dataHolder.GetSprite(note.id);
    }

    void OnMouseDown(){
        if(!(note.id == null || note.id.Length < 1)){
            dataHolder.modifyInventory(note.id, 1, DataModifyType.ADD);
            note.id = null;
            inventoryManager.refreshInventory(note.requiredPotion);
            potionPrefab.enabled = false;
        }

        string current = inventoryManager.currentSelectedItem;
        inventoryManager.refreshInventory(note.requiredPotion);
        if(current == null || current.Length < 1){
            crateView.OnEnable(); 
            refreshBigNote();
            return;
        }
        

        note.id = current;
        potionPrefab.enabled = true;
        potionPrefab.sprite = dataHolder.GetSprite(note.id);
        dataHolder.modifyInventory(note.id, 1, DataModifyType.REMOVE);
        refreshBigNote();
        inventoryManager.refreshInventory(note.requiredPotion);
        inventoryManager.currentSelectedItem = null;
        crateView.OnEnable(); 
    }
}

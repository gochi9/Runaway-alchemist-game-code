using UnityEngine;
using TMPro;
using System;

public class SlotData : MonoBehaviour
{
    public string holdingItemID;
    public TextMeshPro textMeshPro;
    private InventoryManager inventoryManager;
    public string itemTitle;
    public string itemDescription;
    public TooltipController tooltipController;
    public DataHolder dataHolder;

    void Awake(){
        try{
            inventoryManager = transform.parent.GetComponent<InventoryManager>();
            tooltipController = transform.parent.parent.parent.GetComponent<TooltipController>();
            dataHolder = transform.parent.parent.parent.GetComponent<DataHolder>();
        }
        catch(Exception){}

        if(inventoryManager == null)
            inventoryManager = FindFirstObjectByType<InventoryManager>();

        if(tooltipController == null)
            tooltipController = FindFirstObjectByType<TooltipController>();

        if(dataHolder == null)
            dataHolder = FindFirstObjectByType<DataHolder>();
    }

    public void OnMouseDown(){
        if(holdingItemID.Equals(inventoryManager.currentSelectedItem))
            inventoryManager.currentSelectedItem = null;
        else{
            inventoryManager.destroyIcon();
            inventoryManager.currentSelectedItem = holdingItemID;
        }
    }

    void OnMouseOver(){
        try{
            Cursor.SetCursor(AssetsHolder.cursor2, Vector2.zero, CursorMode.Auto);
        bool isPotion = dataHolder.GetItemType(holdingItemID) == ItemType.POTION;
        string newId = isPotion ? removeRarity(holdingItemID) : holdingItemID, rarity = inventoryManager.dataHolder.getRarity(holdingItemID);

        if(itemTitle == null || itemTitle.Length < 1){
            
            itemTitle = dataHolder.itemNamesAndDescriptions[newId].name;
            Debug.Log("No Name");    
        }

        if(itemDescription == null || itemDescription.Length < 1){
            itemDescription = dataHolder.itemNamesAndDescriptions[newId].lore;
            Debug.Log("No Lore");    
        }

        tooltipController.showTooltip(rarity[0].ToString() + rarity.Substring(1).ToLower() + " " + itemTitle, itemDescription);
        }
        catch(Exception){}
    }

    void OnMouseExit(){
        Cursor.SetCursor(AssetsHolder.cursor1, Vector2.zero, CursorMode.Auto);
        tooltipController.hide();
    }

    void OnDisable(){
        OnMouseExit();
    }

    public static string removeRarity(string input){
        int lastUnderscoreIndex = input.LastIndexOf('_');
        
        if (lastUnderscoreIndex == -1)
            return input;
        
        return input.Substring(0, lastUnderscoreIndex);
    }
}

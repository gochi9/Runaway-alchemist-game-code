using System;
using System.Collections.Generic;
using UnityEngine;

public class FermentationView : MonoBehaviour
{


    public SpriteRenderer[] jarFull, jarComplete;
    public InventoryManager inventoryManager;
    public int state;
    public Dictionary<string, float> items = new ();
    public int lastSeenDay;
    public DayCounter dayCounter;
    public DataHolder dataHolder;
    public bool first;

    void OnEnable(){
        if(state == 1 && dayCounter.currentDay != lastSeenDay)
            ++state;

        lastSeenDay = dayCounter.currentDay;

        refreshView();
    }

    public void addItem(string item, float amount){
        if(state == 2)
            return;

        float inventoryAmount = dataHolder.getItemAmount(item);

        if(amount > inventoryAmount)
            amount = inventoryAmount;

        float currentAmount = amount + getAmount(item);

        if(!(currentAmount > 0.0f))
            return;

        if(state == 0){
            ++state;
            refreshView();
        }

        if(!items.ContainsKey(item))
            items.Add(item, currentAmount);
        else
            items[item] += currentAmount;

        dataHolder.modifyInventory(item, amount, DataModifyType.REMOVE);
        inventoryManager.refreshInventory();
    }

    public void retrieveItems(){
        if(state != 2)
            return;

        foreach(string key in items.Keys)
            dataHolder.modifyInventory("FERMENTED_" + key, items[key] * 0.9f, DataModifyType.ADD);

        state = 0;
        items.Clear();
        refreshView();
    }

    private float getAmount(string item){
        try{
            return items[item];
        }
        catch(Exception){
            return 0.0f;
        }
    }

    void refreshView(){
        foreach(SpriteRenderer spriteRenderer in jarFull)
            spriteRenderer.enabled = false;

        foreach(SpriteRenderer spriteRenderer in jarComplete)
            spriteRenderer.enabled = false;

        if(state >= 1)
            foreach(SpriteRenderer spriteRenderer in jarFull)
                spriteRenderer.enabled = true;

        if(state >= 2)
            foreach(SpriteRenderer spriteRenderer in jarComplete)
                spriteRenderer.enabled = true;    
    }


}

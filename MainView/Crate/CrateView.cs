using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateView : MonoBehaviour
{
    public List<string> potions = new ();
    public List<DialogueSO> descriptions = new ();
    public List<int> mild, intermediate, masterful, miracle;
    public List<note> notes = new ();
    public InventoryManager inventoryManager;
    public BigNote bigNote;
    public DataHolder dataHolder;

    public void OnEnable(){
        foreach(note note in notes)
            note.refreshSprite();

        inventoryManager.dataHolder = dataHolder;
        inventoryManager.refreshInventory();
        bigNote.gameObject.SetActive(false);
    }

    public void createBigNote(note note){
        bigNote.note = note;
        bigNote.gameObject.SetActive(true);
        bigNote.refreshBigNote();
        inventoryManager.refreshInventory(note.requiredPotion);
    }
    

    public void selectNewNotes(){
        List<int> alreadySelected = new ();
        int amount = 0;
        int alreadyDiscovered = dataHolder.discovered_potions.Count;
        foreach(note note in notes){
            if(note.id != null && note.id.Length > 0)
                amount += note.getAmount();
            
            int random = getRandomNumber(alreadySelected, alreadyDiscovered--);
            note.id = null;
            note.requiredPotion = potions[random];
            note.description = descriptions[random].lines[UnityEngine.Random.Range(0, 10)];
            note.mild_rarity = mild[random];
            note.intermediate_rarity = intermediate[random];
            note.masterful_rarity = masterful[random];
            note.miracle_rarity = miracle[random];
        }

        dataHolder.money += amount;
    }

    private int getRandomNumber(List<int> alreadySelected, int alreadyDiscovered){
        int random = UnityEngine.Random.Range(0, potions.Count);

        if(alreadySelected.Contains(random) || (alreadyDiscovered > 0 && !dataHolder.discovered_potions.Contains(potions[random])))
            return getRandomNumber(alreadySelected, --alreadyDiscovered);
        else
            return random;
    }

}

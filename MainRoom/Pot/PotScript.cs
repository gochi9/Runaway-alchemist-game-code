using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PotScript : MonoBehaviour
{
    public SpriteRenderer upperCircle, leftCircle, rightCircle, lowerCircle, upleftCircle, upRightCircle, lowerLeftCircle, lowerRightCircle;
    public GameObject dark_prefab;
    public Transform dark_container;
    public float potionProgress, failProgress;
    public PotionData currentPotion;
    public InventoryManager inventoryManager;
    public DataHolder dataHolder;
    public PointMove pointMove;
    private float flashTimerCircle, flashTimerFail;
    private Camera mainCamera;
    private float potComplete;
    private bool usingSpoon;
    public DialogueManager dialogueManager;

    void Awake(){
        setOpacity(upperCircle, 0);
        setOpacity(leftCircle, 0);
        setOpacity(rightCircle, 0);
        setOpacity(lowerCircle, 0);
        setOpacity(upleftCircle, 0);
        setOpacity(upRightCircle, 0);
        setOpacity(lowerLeftCircle, 0);
        setOpacity(lowerRightCircle, 0);
        mainCamera = Camera.main;
    }

    public void updateCircle(float x, float y){
        if (y > 0){
            setOpacity(upperCircle, y);
            setOpacity(lowerCircle, 0);
        }
        else{
            setOpacity(upperCircle, 0);
            setOpacity(lowerCircle, y);
        }

        if (x > 0){
            setOpacity(leftCircle, 0);
            setOpacity(rightCircle, x);
        }
        else{
            setOpacity(leftCircle, x);
            setOpacity(rightCircle, 0);
        }
    }

    private Vector2 lastMouseMove;

    public void FixedUpdate(){
        updateDark();
        float flashSpeed = Mathf.Lerp(1f, 10f, potionProgress);
        flashTimerCircle += Time.fixedDeltaTime * flashSpeed;
        float flashOpacity = (Mathf.Sin(flashTimerCircle) + 1f) / 2f;

        setFlashingOpacity(upleftCircle, flashOpacity, potionProgress);
        setFlashingOpacity(upRightCircle, flashOpacity, potionProgress);
        setFlashingOpacity(lowerLeftCircle, flashOpacity, potionProgress);
        setFlashingOpacity(lowerRightCircle, flashOpacity, potionProgress);

        if(!usingSpoon)
            return;

        Vector2 old = lastMouseMove;
        if(lastMouseMove == null){
            lastMouseMove = Input.mousePosition;
            return;
        }

        if(pointMove.moving){
            OnMouseUp();
            return;
        }

        lastMouseMove = Input.mousePosition;

        if(old == lastMouseMove)
            return;

        if((potComplete += 0.01f) <= 1f)
            return;
        
        createdPotion();
    }

    private void updateDark(){
        foreach (Transform child in dark_container) 
            Destroy(child.gameObject);

        int numDarks = Mathf.FloorToInt(failProgress * 10);

        for (int i = 0; i < numDarks; i++) {
            GameObject dark = Instantiate(dark_prefab, dark_container);
            dark.transform.localPosition = new Vector3(Random.Range(-0.10f, 0.10f), Random.Range(-0.10f, 0.10f), -0.01f * i);
        }
    }

    public void OnMouseDown(){
        string id = inventoryManager.currentSelectedItem;
        float amount = dataHolder.getItemAmount(id);
        if(id == null || id.Length < 1)
            return;

        if(id.Equals("SPOON")){
            if(currentPotion == null)
                return;

            usingSpoon = true;
            return;
        }

        if(amount <= 0f){
            inventoryManager.currentSelectedItem = null;
            return;
        }

        float remove = inventoryManager.selectedDropAmount;
        remove = remove > amount ? amount : remove;
        dataHolder.modifyInventory(id, remove, DataModifyType.REMOVE);
        Vector3 mouseScreenPosition = Input.mousePosition;
        dataHolder.showFloatingText("-" + PotScript.formatDouble(remove) + " g", mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y + 100f, mainCamera.nearClipPlane)));   

        float toMoveX = dataHolder.moveDirectionsPer0_1g[id][0];
        float toMoveY = dataHolder.moveDirectionsPer0_1g[id][1];
        Debug.Log(toMoveX + " " + toMoveY + " " + remove + " " + (toMoveX * remove));
        pointMove.movePoint(toMoveX * remove, toMoveY * remove);

        inventoryManager.refreshInventory();
    }

    public void OnMouseUp(){
        if(!usingSpoon)
            return;

        potComplete = 0f;
        usingSpoon = false;
        inventoryManager.currentSelectedItem = null;
    }

    void OnDisable(){
        usingSpoon = false;
    }

    private void setOpacity(SpriteRenderer spriteRenderer, float value){
        Color color = spriteRenderer.color;
        color.a = Mathf.Abs(value);
        spriteRenderer.color = color;
        setText(spriteRenderer, value);
    }
    
    public string potionId;
    private void createdPotion(){
        potionId = currentPotion.potionID + calculateRarity(potionProgress);

        OnMouseUp();
        dialogueManager.potionPlaceholder = getSimplePotion(currentPotion.potionID);
        dialogueManager.rarityPlaceholder = getSimpleRarity(calculateRarity(potionProgress));
        
        if(dataHolder.hasDiscoveredPotion(potionId))
            dialogueManager.startDialogue("COMPLETE_POTION");
        else
            dialogueManager.startDialogue("DISCOVER_POTION");
    }

    public void finishPotion(){
        string format = dataHolder.extractPotion(potionId);
        if(!dataHolder.discovered_potions.Contains(format))
            dataHolder.discovered_potions.Add(format);

        int amount = (int)dataHolder.getItemAmount(potionId);
        dataHolder.inventoryData.Remove(potionId);
        dataHolder.inventoryData.Add(potionId, amount + 1);
        dataHolder.spawnBottle(potionProgress);
        resetPotion();
    }

    public void resetPotion(){
        potionProgress = 0f;
        failProgress = 0f;
        currentPotion = null;
        flashTimerCircle = 0f;
        flashTimerFail = 0f;
        pointMove.targetx = 0.0f;
        pointMove.targety = 0.0f;
        pointMove.gameObject.transform.position = new Vector3(0, 0, -0.1f);
        potionId = null;
        OnMouseUp();
        Awake();
        pointMove.moving = false;
    }

    public void tastePotion(){
        DialogueSO dialogueSO = ScriptableObject.CreateInstance<DialogueSO>();
        dialogueSO.lines.Add(currentPotion.tasteText);
        dialogueManager.potionPlaceholder = getSimplePotion(currentPotion.potionID);
        dialogueManager.rarityPlaceholder = getSimpleRarity(calculateRarity(potionProgress));
        dialogueSO.nextDialogue = dialogueManager.GetDialogueSO("COMPLETE_POTION");
        dialogueManager.startDialogue(dialogueSO);
    }

    private void setFlashingOpacity(SpriteRenderer spriteRenderer, float value, float value2){
        Color color = spriteRenderer.color;
        color.a = value * potionProgress;
        spriteRenderer.color = color;
        setText(spriteRenderer, value2);
    }

    private void setText(SpriteRenderer spriteRenderer, float value){
        try{
            TextMeshPro textMeshPro = spriteRenderer.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>();
            textMeshPro.text = formatDouble(Mathf.Abs(value * 100)) + "%";
        }
        catch(Exception){}   
    }

    private string calculateRarity(float value){
        if(value > 0.5f && value <= 0.90f)
            return "_INTERMEDIATE";
        else if(value > 0.90f && value <= 0.995f)
            return "_MASTERFUL";
        else if(value > 0.995f)
            return "_MIRACLE";

        return "_MILD";
    }

    public static string formatDouble(float value){
        return string.Format(CultureInfo.InvariantCulture, "{0:0.0}", value);
    }

    private static string getSimplePotion(string input){
        string trimmedInput = input.Replace("POTION_OF_", "");
        string[] words = trimmedInput.Split('_');

        for (int i = 0; i < words.Length; i++)
            if (words[i].Length > 0)
                words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i].ToLower());
            
        return string.Join(" ", words);
    }

    private static string getSimpleRarity(string input){
        string trimmedInput = input.TrimStart('_');

        if (trimmedInput.Length > 0)
            trimmedInput = char.ToUpper(trimmedInput[0]) + trimmedInput.Substring(1).ToLower();

        return trimmedInput;
    }

}
using System;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextInputField : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public GameObject child;
    public TMP_InputField inputField;

    void Awake(){
        child.SetActive(false);
    }

    void LateUpdate(){
        if(Input.GetKeyDown(KeyCode.Return))
            changeAmount();
    }

    public void OnDisable(){
        if(child != null && !child.IsDestroyed())
            child.SetActive(false);
    }

    public void changeAmount(){
        float amount;
        try{
            string normalizedInput = inputField.text.Replace(',', '.');

            if (float.TryParse(normalizedInput, NumberStyles.Float, CultureInfo.InvariantCulture, out amount)){}
          //  amount = float.Parse(inputField.text.Replace(".", ","));
        } catch (Exception e){
            amount = inventoryManager.selectedDropAmount;
            Debug.Log(e.Message);
            Debug.Log(e);
            Debug.Log("ERROR");
        }

        Debug.Log(amount);
        inventoryManager.selectedDropAmount = amount;
        child.SetActive(false);
    }

    void OnMouseDown(){
        child.SetActive(true);
        inputField.text = inventoryManager.selectedDropAmount.ToString();
    }
}

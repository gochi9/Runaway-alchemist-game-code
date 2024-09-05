using TMPro;
using UnityEngine;

public class ConfirmButton : MonoBehaviour
{
    public TextInputField textInputField;

    void OnMouseDown(){
        textInputField.changeAmount();
    }
}

using UnityEngine;

public class ConfirmCollider : MonoBehaviour
{
    public TextInputField textInputField;

    void OnMouseDown(){
        textInputField.changeAmount();
    }
}

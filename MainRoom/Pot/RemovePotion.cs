using UnityEngine;

public class RemovePotion : MonoBehaviour
{
    public PotScript potScript;

    public void OnMouseDown(){
        potScript.resetPotion();
    }
}

using UnityEngine;

public class PotionData : MonoBehaviour
{
    public string potionID;
    public string tasteText;
    public bool lethal;

    void Awake(){
        potionID = potionID.ToUpper();
    }
}

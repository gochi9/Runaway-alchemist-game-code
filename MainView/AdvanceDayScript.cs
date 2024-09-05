using UnityEngine;

public class AdvanceDayScript : MonoBehaviour
{
    public DayCounter dayCounter;

    public void OnMouseDown(){
        dayCounter.advanceDay();
    }
}

using UnityEngine;

public class ResourcesHolder : MonoBehaviour
{
    public ResourcesSpawner normalSpawner, bonusSpawner;

    public void spawn(){
        if(normalSpawner != null)
            normalSpawner.spawn();

        //check if bonus is active
    }
}

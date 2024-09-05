using System.Collections.Generic;
using UnityEngine;

public class ResourcesSpawner : MonoBehaviour
{

    public int toSpawnMin, toSpawnMax, toSpawnBonusMin, toSpawnBonusMax;
    private List<ResourcesData> resources = new ();

    void Awake(){
        foreach (Transform child in transform){
            ResourcesData resource = child.gameObject.GetComponent<ResourcesData>();

            if(resource != null)
                resources.Add(resource);
        }

        toSpawnMin = Mathf.Max(toSpawnMin, 0);
        toSpawnMax = Mathf.Max(toSpawnMax, 1);
        toSpawnBonusMin = Mathf.Max(toSpawnMin, 0);
        toSpawnBonusMax = Mathf.Max(toSpawnMin, 1);
    }

    public void spawn(){
        if(resources.Count < 1)
            return;

        //Check if bonus is active

        int random = toSpawnMin != toSpawnMax ? Random.Range(toSpawnMin, toSpawnMax + 1) : toSpawnMin;

        // if(random > resources.Count)
        //     random = resources.Count;

        for(int i = 0; i < random; i++){
            ResourcesData resource = resources[Random.Range(0, resources.Count)];
            resource.amount += resource.minAmount != resource.maxAmount ? Random.Range(resource.minAmount, resource.maxAmount) : resource.minAmount;
            resource.refresh();
        }
    }

}
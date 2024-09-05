using System;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class DataHolder : MonoBehaviour
{
    public GameObject floatingTextPrefab;
    public Dictionary<string, float> inventoryData = new ();
    public Dictionary<string, Sprite> itemSprites = new ();
    public List<Sprite> sprites = new ();
    public List<string> names = new ();
    public List<string> discovered_potions = new();
    public List<string> visibleNames = new ();
    public List<string> descriptions = new ();
    public GameObject[] bottles = new GameObject[3];
    public Sprite[] bottlesSprites = new Sprite[3];
    //First value is x, second value is y
    public Dictionary<string, float[]> moveDirectionsPer0_1g = new (); 
    public Dictionary<string, Pair> itemNamesAndDescriptions = new();
    public GameObject randomBottlePlacements;
    private List<BoxCollider2D> placements = new ();
    public int money;

    void Awake(){
        Shuffle(bottles, bottlesSprites);
        Shuffle(bottles, bottlesSprites);

        for(int i = 0; i < names.Count; i++){
            itemNamesAndDescriptions.Add(names[i], new Pair(visibleNames[i], descriptions[i]));
            
            if(!names[i].StartsWith("POTION_OF_")){
                itemSprites.Add(names[i], sprites[i]);
                continue;
            }
            
            itemSprites.Add(names[i] + "_MILD", bottlesSprites[0]);
            itemSprites.Add(names[i] + "_INTERMEDIATE", bottlesSprites[1]);
            itemSprites.Add(names[i] + "_MASTERFUL", bottlesSprites[2]);
            itemSprites.Add(names[i] + "_MIRACLE", bottlesSprites[2]);
        }

        // Normal Ingredients
        moveDirectionsPer0_1g.Add("COMMON_MUSHROOM", new float[]{0.05f, 0.016f});
        moveDirectionsPer0_1g.Add("WEIRD_MOLD", new float[]{0.01f, -0.016f});
        moveDirectionsPer0_1g.Add("LIFE_MOLD", new float[]{0.005f, 0.04f});
        moveDirectionsPer0_1g.Add("POISON_MUSHROOM", new float[]{-0.005f, -0.04f});
        moveDirectionsPer0_1g.Add("IRON_GRASS", new float[]{-0.03f, 0.025f});
        moveDirectionsPer0_1g.Add("MOSS", new float[]{0.025f, 0.005f});
        moveDirectionsPer0_1g.Add("HAPPY_FLOWERS", new float[]{-0.025f, -0.0022f});
        moveDirectionsPer0_1g.Add("MAGIC_WHEAT", new float[]{-0.002f, 0.002f});
        moveDirectionsPer0_1g.Add("WINE_MUSHROOM", new float[]{-0.01f, -0.01f});
        moveDirectionsPer0_1g.Add("STONE_MUSHROOM", new float[]{0.01f, 0.01f});

        // Distilled Ingredients
        moveDirectionsPer0_1g.Add("DISTILLED_COMMON_MUSHROOM", new float[]{0.004f, -0.00014f});
        moveDirectionsPer0_1g.Add("DISTILLED_WEIRD_MOLD", new float[]{-0.001f, 0.014f});
        moveDirectionsPer0_1g.Add("DISTILLED_LIFE_MOLD", new float[]{0.0005f, 0.08f});
        moveDirectionsPer0_1g.Add("DISTILLED_POISON_MUSHROOM", new float[]{-0.0015f, -0.07f});
        moveDirectionsPer0_1g.Add("DISTILLED_IRON_GRASS", new float[]{0.0025f, 0.06f});
        moveDirectionsPer0_1g.Add("DISTILLED_MOSS", new float[]{0.0008f, 0.10f});
        moveDirectionsPer0_1g.Add("DISTILLED_HAPPY_FLOWERS", new float[]{0.0035f, 0.12f});
        moveDirectionsPer0_1g.Add("DISTILLED_MAGIC_WHEAT", new float[]{0.0015f, 0.05f});
        moveDirectionsPer0_1g.Add("DISTILLED_WINE_MUSHROOM", new float[]{0.0008f, 0.02f});
        moveDirectionsPer0_1g.Add("DISTILLED_STONE_MUSHROOM", new float[]{-0.0025f, 0.03f});

        // Grinded Ingredients
        moveDirectionsPer0_1g.Add("GRINDED_COMMON_MUSHROOM", new float[]{0.003f, 0.00010f});
        moveDirectionsPer0_1g.Add("GRINDED_WEIRD_MOLD", new float[]{0.0008f, 0.010f});
        moveDirectionsPer0_1g.Add("GRINDED_LIFE_MOLD", new float[]{-0.0004f, 0.05f});
        moveDirectionsPer0_1g.Add("GRINDED_POISON_MUSHROOM", new float[]{-0.0018f, -0.05f});
        moveDirectionsPer0_1g.Add("GRINDED_IRON_GRASS", new float[]{0.0028f, 0.04f});
        moveDirectionsPer0_1g.Add("GRINDED_MOSS", new float[]{0.0009f, 0.07f});
        moveDirectionsPer0_1g.Add("GRINDED_HAPPY_FLOWERS", new float[]{0.0038f, 0.09f});
        moveDirectionsPer0_1g.Add("GRINDED_MAGIC_WHEAT", new float[]{0.0018f, 0.04f});
        moveDirectionsPer0_1g.Add("GRINDED_WINE_MUSHROOM", new float[]{0.0009f, 0.01f});
        moveDirectionsPer0_1g.Add("GRINDED_STONE_MUSHROOM", new float[]{-0.0028f, 0.02f});

        // Fermented Ingredients    
        moveDirectionsPer0_1g.Add("FERMENTED_COMMON_MUSHROOM", new float[]{0.002f, -0.00012f});
        moveDirectionsPer0_1g.Add("FERMENTED_WEIRD_MOLD", new float[]{-0.0009f, 0.012f});
        moveDirectionsPer0_1g.Add("FERMENTED_LIFE_MOLD", new float[]{0.0006f, 0.06f});
        moveDirectionsPer0_1g.Add("FERMENTED_POISON_MUSHROOM", new float[]{-0.0022f, -0.06f});
        moveDirectionsPer0_1g.Add("FERMENTED_IRON_GRASS", new float[]{0.0032f, 0.05f});
        moveDirectionsPer0_1g.Add("FERMENTED_MOSS", new float[]{0.0012f, 0.08f});
        moveDirectionsPer0_1g.Add("FERMENTED_HAPPY_FLOWERS", new float[]{0.0042f, 0.10f});
        moveDirectionsPer0_1g.Add("FERMENTED_MAGIC_WHEAT", new float[]{0.0022f, 0.05f});
        moveDirectionsPer0_1g.Add("FERMENTED_WINE_MUSHROOM", new float[]{0.0012f, 0.03f});
        moveDirectionsPer0_1g.Add("FERMENTED_STONE_MUSHROOM", new float[]{-0.0032f, 0.04f});


        foreach(Transform child in randomBottlePlacements.transform)
            placements.Add(child.GetComponent<BoxCollider2D>());
    }

    public float getItemAmount(string id){
        try{
            return inventoryData[id];
        }
        catch(Exception){
            return 0;
        }
    }

    public void modifyInventory(string id, float amount, DataModifyType type){
        if(amount == 0f)
            return;

        float currentAmount = getItemAmount(id);

        switch(type){
            case DataModifyType.ADD:
                currentAmount += amount;
                break;
            case DataModifyType.REMOVE:
                currentAmount -= amount;
                break;
            case DataModifyType.SET:
                currentAmount = amount;
                break;
        }

        if(currentAmount <= 0){
            inventoryData.Remove(id);
            return;
        }

        if(!inventoryData.ContainsKey(id))
            inventoryData.Add(id, currentAmount);
        else
            inventoryData[id] = currentAmount >= 0 ?currentAmount : 0;
    }

    public void spawnBottle(float value){
        BoxCollider2D box = placements[Random.Range(0, placements.Count)];
        
        GameObject bottle = Instantiate(bottles[calculateRarity(value)], Vector3.zero, Quaternion.identity);
        float bottleHeight = bottle.GetComponent<SpriteRenderer>().bounds.size.y;
        
        float spawnX = Random.Range(box.bounds.min.x, box.bounds.max.x);
        float spawnY = box.bounds.center.y + (bottleHeight / 2);

        bottle.transform.position = new Vector3(spawnX, spawnY, -0.25f + ((spawnY - -0.75f) / (0.6f - -0.75f)) * (-0.1f - -0.25f));
        
        bottle.transform.parent = box.transform.parent;
    }

    public void showFloatingText(string message, Vector3 position){
        if(floatingTextPrefab == null)
            return;

        GameObject instance = Instantiate(floatingTextPrefab);
        position = new Vector3(position.x, position.y, -8);
        instance.transform.position = position;

        FloatingText floatingText = instance.GetComponent<FloatingText>();
        if (floatingText != null)
            floatingText.setText(message);
    }

    public Sprite GetSprite(string id){
        try{
            return itemSprites[id];
        }
        catch(Exception){
            return null;
        }
    }

    public ItemType GetItemType(string id){
        if(id == null || id.Length < 1)
            return ItemType.NULL;

        if(id.StartsWith("DISTILLED_") || id.StartsWith("GRINDED_") || id.StartsWith("FERMENTED_"))
            return ItemType.PREPARED;
        
        if(id.StartsWith("POTION_OF_"))
            return ItemType.POTION;

        return ItemType.RAW;
    }

    public bool hasDiscoveredPotion(string potion_id){
        return discovered_potions.Contains(extractPotion(potion_id));
    }

    public string getRarity(string id){
        int lastUnderscoreIndex = id.LastIndexOf('_');
        
        if (lastUnderscoreIndex == -1)
            return null;
        
        return id.Substring(lastUnderscoreIndex + 1);
    }

    public string extractPotion(string potion_id){
        int lastUnderscoreIndex = potion_id.LastIndexOf('_');
        if (lastUnderscoreIndex != -1)
            return potion_id.Substring(0, lastUnderscoreIndex);
        
        return potion_id;
    }

    private int calculateRarity(float value){
        if(value > 0.5f && value <= 0.90f)
            return 1;
        else if(value > 0.90f && value <= 0.995f)
            return 2;
        else if(value > 0.995f)
            return 2;

        return 0;
    }

    //Stole this from stackoverflow
    public static void Shuffle<T, K> (T[] array, K[] array2)
    {
        int n = array.Length;
        while (n > 1) 
        {
            int k = Random.Range(0, n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;

            K temp2 = array2[n];
            array2[n] = array2[k];
            array2[k] = temp2;
        }
    }
}

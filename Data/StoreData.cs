using UnityEngine;
using System.Collections.Generic;

public class StoreData : MonoBehaviour{
    private DataHolder dataHolder;
    private DayCounter dayCounter;
    private DialogueManager dialogueManager;
    public FermentationView fermentationView;
    public List<FrameHolder> frameHolders;
    //public List<ResourcesSpawner> resourcesSpawners = new();

    void Awake(){
        dataHolder = GetComponent<DataHolder>();
        dayCounter = GetComponent<DayCounter>();
        dialogueManager = GetComponent<DialogueManager>();
    }

    public void saveGame(){
        PlayerPrefs.SetInt("currentDay", dayCounter.currentDay);
        PlayerPrefs.SetInt("attention", dayCounter.attention);

        saveDictionary("inventoryData", dataHolder.inventoryData);
        saveList("discovered_potions", dataHolder.discovered_potions);
        PlayerPrefs.SetInt("money", dataHolder.money);

        PlayerPrefs.SetInt("lastSeenDay", fermentationView.lastSeenDay);
        PlayerPrefs.SetInt("state", fermentationView.state);
        saveDictionary("fermentationItems", fermentationView.items);

        foreach(FrameHolder frameHolder in frameHolders)
            PlayerPrefs.SetInt(frameHolder.dialogueID, frameHolder.firstEnable ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void loadGame(){
        if (!PlayerPrefs.HasKey("currentDay"))
            return;

        dayCounter.currentDay = PlayerPrefs.GetInt("currentDay");
        dayCounter.attention = PlayerPrefs.GetInt("attention");

        dataHolder.inventoryData = loadDictionary("inventoryData");
        dataHolder.discovered_potions = loadList("discovered_potions");
        dataHolder.money = PlayerPrefs.GetInt("money");

        fermentationView.lastSeenDay = PlayerPrefs.GetInt("lastSeenDay");
        fermentationView.state = PlayerPrefs.GetInt("state");
        fermentationView.items = loadDictionary("fermentationItems");

        frameHolders[0].firstEnable = PlayerPrefs.GetInt("MAINHALL") == 1;
        frameHolders[1].firstEnable = PlayerPrefs.GetInt("SECONDHALL") == 1;
        frameHolders[2].firstEnable = PlayerPrefs.GetInt("MAINROOM") == 1;
        frameHolders[3].firstEnable = PlayerPrefs.GetInt("POTVIEW") == 1;
        frameHolders[4].firstEnable = PlayerPrefs.GetInt("MORTARVIEW") == 1;
        frameHolders[5].firstEnable = PlayerPrefs.GetInt("FERMENTATIONVIEW") == 1;
        frameHolders[6].firstEnable = PlayerPrefs.GetInt("DISTILLERVIEW") == 1;
        frameHolders[7].firstEnable = PlayerPrefs.GetInt("RITUALROOM") == 1;
        frameHolders[8].firstEnable = PlayerPrefs.GetInt("CELLARROOM") == 1;

        dayCounter.continueAdvanceDay(true);
        dayCounter.forceFadeIn();
    }

    void saveDictionary(string key, Dictionary<string, float> dictionary){
        string json = JsonUtility.ToJson(new Serialization<string, float>(dictionary));
        PlayerPrefs.SetString(key, json);
    }

    Dictionary<string, float> loadDictionary(string key){
        string json = PlayerPrefs.GetString(key, "{}");
        return JsonUtility.FromJson<Serialization<string, float>>(json).ToDictionary();
    }

    void saveList(string key, List<string> list){
        string json = JsonUtility.ToJson(new Serialization<string>(list));
        PlayerPrefs.SetString(key, json);
    }

    List<string> loadList(string key){
        string json = PlayerPrefs.GetString(key, "[]");
        return JsonUtility.FromJson<Serialization<string>>(json).ToList();
    }
}

[System.Serializable]
public class Serialization<T>{
    public List<T> target;

    public Serialization(List<T> target){
        this.target = target;
    }

    public List<T> ToList(){
        return target;
    }
}

[System.Serializable]
public class Serialization<TKey, TValue>{
    public List<TKey> keys;
    public List<TValue> values;

    public Serialization(Dictionary<TKey, TValue> target){
        keys = new List<TKey>(target.Keys);
        values = new List<TValue>(target.Values);
    }

    public Dictionary<TKey, TValue> ToDictionary(){
        Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
        for (int i = 0; i < keys.Count; i++){
            dict[keys[i]] = values[i];
        }
        return dict;
    }
}

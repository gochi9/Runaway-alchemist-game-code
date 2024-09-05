using System;
using UnityEngine;

public class note : MonoBehaviour
{
    private CrateView crateView;
    public Sprite v,x;
    public SpriteRenderer child;
    public string id, requiredPotion, description;
    public int mild_rarity, intermediate_rarity, masterful_rarity, miracle_rarity;

    void Awake(){
        crateView = transform.parent.GetComponent<CrateView>();
    }

    public void OnEnable(){
        refreshSprite();
    }

    public void refreshSprite(){
        try{
            if(id == null || id.Length < 1)
            child.sprite = x;
        else
            child.sprite = v;
        }
        catch(Exception e){
            Debug.Log(e);
            Debug.Log(e.Message);
            Debug.Log(gameObject.name);
        }
    }

    public void OnMouseDown(){
        crateView.createBigNote(this);
    }

    public int getAmount(){
        string rarity = crateView.dataHolder.getRarity(id);
        switch(rarity){
            case "MILD":
                return mild_rarity;
            case "INTERMEDIATE":
                return intermediate_rarity;
            case "MASTERFUL":
                return masterful_rarity;
            case "MIRACLE":
                return miracle_rarity;
            default:
                return mild_rarity;
        }
    }
}

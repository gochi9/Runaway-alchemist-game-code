using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayCounter : MonoBehaviour
{

    public int currentDay;
    public List<int> inspectionDays;
    public GameObject main_room_candle, main_scene;
    public AudioSource blowOutSound, lightUpSound, music;
    private Light2D light;
    private CandleLightEffect candleLightEffect;

    private AssetsHolder assetsHolder;
    private bool fadeOut, fadeOut2, fadeIn;
    private long  fadeOut2Delay, fadeInDelay;
    public bool CHANGING_DAY;
    public int attention;
    private StoreData storeData;
    public CrateView crateView;

    void Start(){
        assetsHolder = GetComponent<AssetsHolder>();
        storeData = GetComponent<StoreData>();

        light = main_room_candle.GetComponent<Light2D>();
        candleLightEffect = main_room_candle.GetComponent<CandleLightEffect>();
    }

    void FixedUpdate(){
        if(!CHANGING_DAY)
            return;

        if(fadeOut)
            animationFadeOut();
        
        else if(fadeOut2)
            animationFadeOut2();
        
        else if(fadeIn)
            animationFadeIn();
    }

    public void advanceDay(){
        currentDay++;
        
        CHANGING_DAY = true;
        crateView.selectNewNotes();
        foreach (BoxCollider2D collider in main_scene.GetComponentsInChildren<BoxCollider2D>(true))
            collider.enabled = false;

        GetComponent<Collider2D>().enabled = true;
        candleLightEffect.enabled = false;
        fadeOut = true;
        //Play some fade out effect
        //Wait a seconds
        //Fade in effect
        //Start next day
    }

    public void continueAdvanceDay(bool onLoad){
        if(!onLoad)
            storeData.saveGame();

        if(inspectionDays.Contains(currentDay))
            inspectionDay();
        else
            normalDay();
    }

    private void normalDay(){
        assetsHolder.resourcesHolders.ForEach(resourcesHolder => resourcesHolder.spawn());
        //Get new orders, spawns npcs at the door
    }
    private void inspectionDay(){

    }

    private void animationFadeOut(){
        if(candleLightEffect == null || light == null){
            fadeOut = false;
            fadeIn = false;
            return;
        }
    
        first = true;
        light.pointLightInnerRadius -= light.pointLightInnerRadius >= 0.5f ? (!fast ? 0.08f : 0.1f) : 0.0f;
        light.pointLightOuterRadius -= light.pointLightOuterRadius > 2.1f ? (!fast ? 0.2f : 0.5f) : 0.0f;
        fadeOut = /* light.pointLightInnerRadius >= 0.0f || */ light.pointLightOuterRadius >= 2.1f;
        music.volume -= music.volume > 0.0f ? 0.04f : 0.0f;

        if(fadeOut)
            return;

        fadeOut2 = true;
        fadeOut2Delay = DateTimeOffset.Now.ToUnixTimeMilliseconds() + (!fast ? 1000 : 0);
    }

    private void animationFadeOut2(){
        if(fadeOut2Delay >= DateTimeOffset.Now.ToUnixTimeMilliseconds())
            return;
        
        fadeOut2 = false;

        if(!fast)
            blowOutSound.Play();

        light.pointLightInnerRadius = 0.0f;
        light.enabled = false;
        
        continueAdvanceDay(false);
        fadeIn = true;
        fadeInDelay = DateTimeOffset.Now.ToUnixTimeMilliseconds() + (!fast ? 2500 : 100);
    }

    private bool first, first2;
    private void animationFadeIn(){
        long diff = fadeInDelay - DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if(diff >= 0){
            if(diff <= 1100 && !fast && first){
                lightUpSound.Play();
                first = false;
            }

            return;    
        }

        light.enabled = true;
        light.pointLightOuterRadius += light.pointLightOuterRadius < candleLightEffect.minOuterRadius ? (!fast ? 0.1f : 0.5f) : 0.0f;
        light.pointLightInnerRadius += light.pointLightInnerRadius < candleLightEffect.minInnerRadius ? (!fast ? 0.04f : 0.5f) : 0.0f;

        fadeIn = light.pointLightOuterRadius < candleLightEffect.minOuterRadius || light.pointLightInnerRadius < candleLightEffect.minInnerRadius;
        music.volume += music.volume < 0.5f ? 0.04f : 0.0f;

        if(fadeIn)
            return;

        if(!music.isPlaying)
            music.Play();
            
        candleLightEffect.enabled = true;
        
        foreach (BoxCollider2D collider in main_scene.GetComponentsInChildren<BoxCollider2D>(true))
            collider.enabled = true;

        fast = false;
        GetComponent<Collider2D>().enabled = false;
        CHANGING_DAY = false;
        //Give player control
    }

    public void forceFadeIn(){
        CHANGING_DAY = true;
        fadeIn = true;
        first = true;
        crateView.selectNewNotes();
        fadeInDelay = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 1200;
    }

    private bool fast = false;

    void OnMouseDown(){
        if(!CHANGING_DAY || currentDay <= 1 || !(fadeIn || fadeOut))
            return;

        fast = true;
    }

}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public List<AudioSource> musicSource = new();
    public List<AudioSource> sfxSource = new ();
    public Slider musicSlider;
    public Slider sfxSlider;

    void Awake(){
        musicSlider.value = 1f;
        sfxSlider.value = 1f;

        musicSlider.onValueChanged.AddListener(setMusicVolume);
        sfxSlider.onValueChanged.AddListener(setSFXVolume);
    }

    void setMusicVolume(float volume){
        musicSource.ForEach(source => source.volume = volume);
    }

    void setSFXVolume(float volume){
        sfxSource.ForEach(source => source.volume = volume);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    [Header("Controls")]
    [SerializeField] GameObject screenContainer;
    public int screensNumber;
    int actualScreen = 0;
    void Start()
    {
        float currentVolume;
        // Obtener el volumen actual del AudioMixer
        audioMixer.GetFloat("musicVolume", out currentVolume);
        musicSlider.value = Mathf.Pow(10, currentVolume / 20);
        audioMixer.GetFloat("sfxVolume", out currentVolume);
        sfxSlider.value = Mathf.Pow(10, currentVolume / 20);
    }

    
    void Update()
    {
        
    }

    public void MusicVolume()
    {
        if (musicSlider.value == 0) audioMixer.SetFloat("musicVolume", -80);
        else audioMixer.SetFloat("musicVolume", 20f* Mathf.Log10(musicSlider.value));
    }

    public void SFXVolume()
    {
        if (sfxSlider.value == 0) audioMixer.SetFloat("sfxVolume", -80);
        else audioMixer.SetFloat("sfxVolume", 20f * Mathf.Log10(sfxSlider.value));
    }

    public void OnArrowClicked(string direction)
    {
        screenContainer.transform.GetChild(actualScreen).gameObject.SetActive(false);

        if (direction == "RIGHT")
        {
            actualScreen++;

            if (actualScreen == screensNumber)
            {
                actualScreen = 0;
            }
        }

        else if (direction == "LEFT")
        {
            actualScreen--;

            if (actualScreen < 0)
            {
                actualScreen = screensNumber - 1;
            }
        }

        screenContainer.transform.GetChild(actualScreen).gameObject.SetActive(true);

    }
}

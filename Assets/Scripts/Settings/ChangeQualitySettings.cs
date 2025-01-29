using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class ChangeQualitySettings : MonoBehaviour
{
    public TMP_Dropdown dropdownQualityLevel, dropdownShadows, dropdownShadowRes, dropdownShadowDistance;

    void Awake()
    {
        UpdateValues();
    }

    void UpdateValues()
    {
        dropdownQualityLevel.value = QualitySettings.GetQualityLevel();
        dropdownShadows.value = (int)QualitySettings.shadows;
        dropdownShadowRes.value = (int)QualitySettings.shadowResolution;

        switch (QualitySettings.shadowDistance)
        {
            case 15:
                dropdownShadowDistance.value = 0;
                break;
            case 20:
                dropdownShadowDistance.value = 1;
                break;
            case 40:
                dropdownShadowDistance.value = 2;
                break;
            case 70:
                dropdownShadowDistance.value = 3;
                break;
            default:
                break;
        }
    }

    public void ChangeQualityLevel()
    {
        QualitySettings.SetQualityLevel(dropdownQualityLevel.value, true);
        UpdateValues();
    }

    public void ChangeShadows()
    {
        switch (dropdownShadows.value)
        {
            case 0:
                QualitySettings.shadows = ShadowQuality.Disable;
                break;
            case 1:
                QualitySettings.shadows = ShadowQuality.HardOnly;
                break;
            case 2:
                QualitySettings.shadows = ShadowQuality.All;
                break;
        }
        UpdateValues();
    }

    public void ChangeShadowResolution()
    {
        switch (dropdownShadowRes.value)
        {
            case 0:
                QualitySettings.shadowResolution = ShadowResolution.Low;
                break;
            case 1:
                QualitySettings.shadowResolution = ShadowResolution.Medium;
                break;
            case 2:
                QualitySettings.shadowResolution = ShadowResolution.High;
                break;
            case 3:
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                break;
        }
        UpdateValues();
    }
    
    public void ChangeShadowDistance()
    {
        switch (dropdownShadowDistance.value)
        {
            case 0:
                QualitySettings.shadowDistance = 15;
                break;
            case 1:
                QualitySettings.shadowDistance = 20;
                break;
            case 2:
                QualitySettings.shadowDistance = 40;
                break;
            case 3:
                QualitySettings.shadowDistance = 70;
                break;
        }
        UpdateValues();
    }
}

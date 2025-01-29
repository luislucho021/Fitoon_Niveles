using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSkinController : MonoBehaviour
{

    public Material matPlayer;
    public Material matHair;
    public Material matSkin;

    public GameObject[] headAccesories;
    public GameObject[] hairStyles;
    public GameObject[] faceAccesories;
    public GameObject[] faceHair;
    public GameObject[] chestClothes;
    public GameObject[] chestAccesories;
    public GameObject[] armsAccesories;
    public GameObject[] legsClothes;
    public GameObject[] feetClothes;

    private void Start()
    {
        RandomizeAccesories();
        RandomizeColors();
    }

    private void RandomizeAccesories()
    {

        if (hairStyles.Length > 0)
        {
            int rnd = Random.Range(-1, hairStyles.Length);
            if (rnd > -1) hairStyles[rnd].SetActive(true);
        }

        if (faceHair.Length > 0)
        {
            int rnd = Random.Range(-20, faceHair.Length);
            if (rnd > -1)
            {
                faceHair[rnd].SetActive(true);
                rnd = Random.Range(-10, faceHair.Length);
                if (rnd > -1) faceHair[rnd].SetActive(true);
            }
        }

        if (headAccesories.Length > 0)
        {
            int rnd = Random.Range(-6, headAccesories.Length);
            if (rnd > -1) headAccesories[rnd].SetActive(true);
        }

        if (faceAccesories.Length > 0)
        {
            int rnd = Random.Range(-10, faceAccesories.Length);
            if (rnd > -1) faceAccesories[rnd].SetActive(true);
            rnd = Random.Range(-12, faceAccesories.Length);
            if (rnd > -1) faceAccesories[rnd].SetActive(true);
        }

        if (armsAccesories.Length > 0)
        {
            int rnd = Random.Range(-2, armsAccesories.Length);
            if (rnd > -1) armsAccesories[rnd].SetActive(true);
        }

        if (chestAccesories.Length > 0)
        {
            int rnd = Random.Range(-8, chestAccesories.Length);
            if (rnd > -1) chestAccesories[rnd].SetActive(true);
        }


        if (chestClothes.Length > 0)
        {
            int rnd = Random.Range(0, chestClothes.Length);
            if (rnd > -1) chestClothes[rnd].SetActive(true);
        }

        if (legsClothes.Length > 0)
        {
            int rnd = Random.Range(0, legsClothes.Length);
            if (rnd > -1) legsClothes[rnd].SetActive(true);
        }

        if (feetClothes.Length > 0)
        {
            int rnd = Random.Range(-1, feetClothes.Length);
            if (rnd > -1) feetClothes[rnd].SetActive(true);
        }

    }

    private void RandomizeColors()
    {
        //Randomize Bot Main Color
        Color col = Color.HSVToRGB(Random.Range(0f, 1f), 0.8f, Random.Range(0.5f, 0.8f));

        foreach (SkinnedMeshRenderer mR in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            foreach (Material mat in mR.materials)
            {
                if (mat.name == (matPlayer.name + " (Instance)")) mat.color = col;
            }

        }

        //Randomize Bot Hair Color
        //col = Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(0.2f, 0.4f), Random.Range(0.2f, 0.5f));
        col = Color.HSVToRGB(Random.Range(0.0f, 1.0f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f));

        foreach (SkinnedMeshRenderer mR in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (mR.material.name == (matHair.name + " (Instance)"))
                mR.material.color = col;
        }

        //Randomize Bot Skin Color
        //col = Color.HSVToRGB(Random.Range(0.03f, 0.08f), Random.Range(0.3f, 0.7f), Random.Range(0.4f, 1.0f));
        col = Color.HSVToRGB(Random.Range(0.03f, 0.06f), Random.Range(0.3f, 0.6f), Random.Range(0.6f, 1.0f));

        foreach (SkinnedMeshRenderer mR in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (mR.material.name == (matSkin.name + " (Instance)"))
                mR.material.color = col;
        }
    }

}

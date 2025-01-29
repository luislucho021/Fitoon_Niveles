using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSkins : MonoBehaviour
{
    GameObject skin;

    //Indices de cada elemento del personaje
    public Dictionary<string, int> botSkinData;

    private void Awake()
    {
        botSkinData = new Dictionary<string, int>();

        if (RaceManager.Instance.numberOfRace == 1)
        {
            RandomizeSkin();
            RandomizeObject("Hair", 0);
            RandomizeObject("Shirt", 1);
            RandomizeObject("Pants", 2);
            RandomizeObject("Shoes", 3);
        }
    }

 

    void RandomizeSkin()
    {
        int rand = Random.Range(0, transform.childCount);
        skin = transform.GetChild(rand).gameObject;
        skin.SetActive(true);

        botSkinData.Add("Skin", rand);
    }

    void RandomizeObject(string type, int index)
    {
        Transform container = skin.transform.GetChild(index);
        int rand = Random.Range(0, container.childCount);

        if(container.childCount != 0) container.GetChild(rand).gameObject.SetActive(true);

        botSkinData.Add(type, rand);
    }
}

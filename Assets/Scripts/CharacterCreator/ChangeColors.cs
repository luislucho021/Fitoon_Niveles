using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColors : MonoBehaviour
{
    public Material selectedMaterial;

    public void SetSelectedMaterial(string type)
    {
        CharacterItem characterItem = GetComponent<ChangeCharacter>().actualCharacter;
        switch (type)
        {
            case "Hair":
                selectedMaterial = characterItem.hair;
                break;
            case "Skin":
                selectedMaterial = characterItem.skin;
                break;
            case "Top":
                selectedMaterial = characterItem.top;
                break;
            case "Bottom":
                selectedMaterial = characterItem.bottom;
                break;
        }
       // selectedMaterial = GameObject.FindGameObjectWithTag(type).GetComponent<SkinnedMeshRenderer>().sharedMaterial;
    }
    public void ChangeColor(Color color)
    {
        selectedMaterial.color = color;
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterItem", menuName = "ScriptableObjects/CharacterItem", order = 1)]
[System.Serializable]
public class CharacterItem : ScriptableObject
{
    public string characterName;
    public GameObject prefab;
    public int prefabId;
    public Material hair, skin, top, bottom;
    public Color hairColor, skinColor, topColor, bottomColor;
    public ObjectItem shoes;
}

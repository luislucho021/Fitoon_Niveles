using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataList", menuName = "ScriptableObjects/CharacterDataList", order = 5)]
[System.Serializable]
public class CharacterDataList : ScriptableObject
{
    public CharacterItem[] characters;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string username;
    public int normalCoins;
    public int points;
    public List<int> purchasedSkins;
    public List<int> purchasedShoes;
    public List<int> purchasedColors;
    public CharacterData playerCharacterData;
    public List<EscenarioItem> scenesPlayed;
}
[System.Serializable]
public class CharacterData
{
    public string characterName;
    public string hairColor, skinColor, topColor, bottomColor; //hex color
    public int shoes;
}


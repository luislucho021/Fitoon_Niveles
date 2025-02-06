using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoader : Object
{
    static CharacterDataList characterDataList;
    public static Character GetCharacter(CharacterData data)
    {
        Character characterStruct = new Character
        {
            prefab = null,
            name = "Error",
        };
		if (characterDataList == null)
        {
            characterDataList = Resources.Load<CharacterDataList>("CharacterDataList");
            if(characterDataList == null ) 
            {
                Debug.LogError("Character Data List Not Found");
                return characterStruct;
            }
		}
        foreach(CharacterItem character in characterDataList.characters)
        {
            if(character.name == data.characterName )
            {
                characterStruct.prefab = character.prefab;
            }
            if(character.shoes.id == data.shoes)
            {
                characterStruct.shoes = character.shoes;
            }
        }
        characterStruct.name = data.characterName;

        ColorUtility.TryParseHtmlString(data.hairColor, out characterStruct.hairColor);
        ColorUtility.TryParseHtmlString(data.skinColor, out characterStruct.skinColor);
        ColorUtility.TryParseHtmlString(data.topColor, out characterStruct.topColor);
		ColorUtility.TryParseHtmlString(data.bottomColor, out characterStruct.bottomColor);

		return characterStruct;
    }
}
public struct Character
{
    public GameObject prefab;
    public string name;
    public ObjectItem shoes;
    public Color hairColor;
    public Color skinColor;
    public Color topColor;
    public Color bottomColor;
}
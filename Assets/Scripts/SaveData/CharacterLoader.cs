using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoader : Object
{
    static CharacterDataList characterDataList;
    public static CharacterItem GetCharacter(CharacterData data)
    {
        if(characterDataList == null)
        {
            characterDataList = Resources.Load<CharacterDataList>("CharacterDataList");
            if(characterDataList == null ) 
            {
                Debug.LogError("Character Data List Not Found");
                return null;
            }
		}
        CharacterItem characterItem = new CharacterItem();
        foreach(CharacterItem character in characterDataList.characters)
        {
            if(character.name == data.characterName )
            {
                characterItem.prefab = character.prefab;
            }
            if(character.shoes.id == data.shoes)
            {
                characterItem.shoes = character.shoes;
            }
        }
        characterItem.name = data.characterName;

        ColorUtility.TryParseHtmlString(data.hairColor, out characterItem.hairColor);
        ColorUtility.TryParseHtmlString(data.skinColor, out characterItem.skinColor);
        ColorUtility.TryParseHtmlString(data.topColor, out characterItem.topColor);
		ColorUtility.TryParseHtmlString(data.bottomColor, out characterItem.bottomColor);

		return characterItem;
    }
}

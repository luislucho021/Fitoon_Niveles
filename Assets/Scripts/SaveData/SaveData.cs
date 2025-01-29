using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;

public class SaveData : MonoBehaviour
{
    public PlayerData player;
    
    private void Awake()
    {
        player  = new PlayerData();
        ReadFromJson();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveToJson();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            ReadFromJson();
        }
    }
    public void SaveToJson()
    {
        string playerData = JsonUtility.ToJson(player);
        string filePath = System.IO.Path.Combine(Application.persistentDataPath,"PlayerData.json");
        System.IO.File.WriteAllText(filePath, playerData);
        //Si está vacío poner un personaje default
        if (player.playerCharacterData is null)
        {
            player.playerCharacterData = new CharacterData();
            player.playerCharacterData.characterName = "Juan";
            player.playerCharacterData.hairColor = "#4D2413";
            player.playerCharacterData.skinColor = "#A87458";
            player.playerCharacterData.topColor = "#B46600";
            player.playerCharacterData.bottomColor = "#4F2F12";
            player.playerCharacterData.shoes = 0;
            playerData = JsonUtility.ToJson(player);
            Debug.Log("No había datos. Creando personaje por defecto.");
        }
        player.username = "Username";
        player.normalCoins = 0;
        player.points = 0;
        System.IO.File.WriteAllText(filePath, playerData);
        Debug.Log("[SAVE] Datos guardados en " + filePath);
    }

    public void ReadFromJson()
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "PlayerData.json");

        try
        {
            string playerData = System.IO.File.ReadAllText(filePath);

            player = JsonUtility.FromJson<PlayerData>(playerData);
            Debug.Log("[SAVE] Datos leidos");
        }
        catch (System.Exception)
        {
            Debug.Log("No existe JSON: Creandolo...");
            SaveToJson();
            ReadFromJson();
        }

    }
}

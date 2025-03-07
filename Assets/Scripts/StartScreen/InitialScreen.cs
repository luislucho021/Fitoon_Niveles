using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class InitialScreen : MonoBehaviour
{
    [SerializeField] GameObject characterContainer;
    [SerializeField] List<CharacterItem> characters;
    CharacterItem actualCharacter;
    [SerializeField] List<ObjectItem> shoes;
    [SerializeField] GameObject treadmillPrefab;
    [SerializeField] TMP_InputField inputName;
    private void Start()
    {
		SaveData.ReadFromJson();
		ReadUsername();
        ReadCharacter();
        ResetScenesPlayed();
        if (RaceManager.Instance != null) RaceManager.Instance.Reset();
    }

    void ReadUsername()
    {
        if (SaveData.player.username == "Username") return;
        else
        {
            inputName.text = SaveData.player.username;
        }
    }
    void ReadCharacter()
    {
        //Leer la skin
        string savedSkin = SaveData.player.playerCharacterData.characterName;
        if (savedSkin == null)
        {
            print("Error: No hay personaje guardado");
            //instanciar a juan
            Instantiate(characters[0].prefab, Vector3.zero, Quaternion.identity, characterContainer.transform);
            return;
        }
        //Buscar personaje
        actualCharacter = characters.Find(character => character.characterName == savedSkin);

        //Instanciar la cinta de correr
        GameObject treadmill = Instantiate(treadmillPrefab, Vector3.zero, Quaternion.identity, characterContainer.transform);

        //Instanciar el personaje como hijo de la cinta
        DestroyImmediate(characterContainer.transform.GetChild(0).gameObject);
        GameObject characterInstance = Instantiate(actualCharacter.prefab, Vector3.zero, Quaternion.identity, treadmill.transform);
        characterInstance.GetComponent<Animator>().SetBool("isRunning", true);
        characterInstance.GetComponent<Outline>().enabled = false;

        UpdateShoes();
        UpdateColors();

        //Colocar a personaje adecuadamente en la cinta
        characterInstance.transform.Rotate(transform.up, 180f);
        characterInstance.transform.position = new Vector3(0, 0.54f, 1.6f);

        //Para alejarlo un poco de la camara
        characterContainer.transform.position = new Vector3(0, 0, -2.91f);
        characterContainer.transform.Rotate(transform.up, 120f);
    }

    void UpdateShoes()
    {
        //Actualizar zapatillas
        GameObject zapatos = GameObject.FindGameObjectWithTag("Shoes");
        SkinnedMeshRenderer renderer = zapatos.GetComponent<SkinnedMeshRenderer>();
        int i = SaveData.player.playerCharacterData.shoes;

        //  Debug.Log($"ANTES: Zapato GO: {zapatos.name}. Mesh rendered: {renderer.sharedMesh}. ActualShoe id: {i}");

        foreach (ObjectItem shoeItem in shoes)
        {
            if (shoeItem.id == i)
            {
                renderer.sharedMesh = shoeItem.mesh;
                renderer.materials = shoeItem.materials;
                break;
            }
        }

        //  Debug.Log($"DESPUES: Zapato GO: {zapatos.name}. Mesh rendered: {renderer.sharedMesh}. ActualShoe id: {i}");
    }

    void UpdateColors()
    {
        Color color = Color.black; //si falla saldr� negro
        if (ColorUtility.TryParseHtmlString(SaveData.player.playerCharacterData.hairColor, out color))
        {
            actualCharacter.hair.color = color;
        }
        if (ColorUtility.TryParseHtmlString(SaveData.player.playerCharacterData.skinColor, out color))
        {
            actualCharacter.skin.color = color;
        }
        if (ColorUtility.TryParseHtmlString(SaveData.player.playerCharacterData.bottomColor, out color))
        {
            actualCharacter.bottom.color = color;
        }
        if (ColorUtility.TryParseHtmlString(SaveData.player.playerCharacterData.topColor, out color))
        {
            actualCharacter.top.color = color;
        }
    }

    public void SaveUsername(string value)
    {
        SaveData.player.username = value;
        SaveData.SaveToJson();
    }

    public void ResetScenesPlayed()
    {
        SaveData.player.scenesPlayed.Clear();
        SaveData.SaveToJson();
    }

    public void StartGame()
    {
        DiscoveryHandler.Passcode = null;
        SceneManager.LoadScene("LobbyScene");
	}
}
